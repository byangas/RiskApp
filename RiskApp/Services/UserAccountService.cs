using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using RiskApp.Repositories;
using RiskApp.Models.User;
using RiskApp.Data;

namespace RiskApp.Services
{
    public class UserAccountService
    {
        private readonly UserAccountRepository accountRepository;
        private readonly RegistrationRepository registrationRepository;
        private readonly ProfileRepository profileRepository;
        private readonly ConnectionManager connectionManager;

        public UserAccountService(UserAccountRepository accountRepository, RegistrationRepository registrationRepository, ProfileRepository profileRepository, ConnectionManager connectionManager)
        {
            this.accountRepository = accountRepository;
            this.registrationRepository = registrationRepository;
            this.profileRepository = profileRepository;
            this.connectionManager = connectionManager;
        }

        public SignInInfo GetSignInInfo(string userName)
        {
            return accountRepository.GetSignInInfo(userName);
        }

        public ClaimsPrincipal GetPrincipal(string userName, string password, string scheme)
        {
            // validate user login and password
            SignInInfo signInInfo = accountRepository.GetSignInInfo(userName);
            if (signInInfo == null || !BCrypt.Net.BCrypt.Verify(password, signInInfo.Password))
            {
                return null;
            }

            string companyId = signInInfo.CompanyId.HasValue ? signInInfo.CompanyId.ToString() : "";
            var claims = new List<Claim>
            {
                // we are storing "name" as Id because we don't really use "Name" as much as the ProfileId and 
                // this will make it so that there is fewer lookups in the DB for this. Optimization based on 
                // previous work where DB lookups to get Ids from DB were killing performance
                new Claim(ClaimTypes.Name, signInInfo.ProfileId.ToString()),
                // userName is Email (for now)
                new Claim(ClaimTypes.Email, userName),
                new Claim("CompanyId", companyId)
            };

            var roles = accountRepository.GetApplicationRoles(signInInfo.AccountId);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsIdentity = new ClaimsIdentity(claims, scheme);

            // setup authentication Principal
            return new ClaimsPrincipal(claimsIdentity);
        }



        public void CreateAccountFromRegistration(Registration registration, string password, string email, string firstName, string lastName, string phone)
        {


            Guid accountId = Guid.NewGuid();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password.Trim());

            using Npgsql.NpgsqlTransaction transaction = connectionManager.BeginTransaction();
            try
            {
                accountRepository.CreateAccount(accountId, hashedPassword);
                //create roles
                accountRepository.CreateRoles(accountId, registration.Roles);
                //check to see if there is an existing profile for this email address.
                // if there is, link the newly created account to this profile
                Profile profile = profileRepository.GetProfile(email);

                if (profile != null)
                {
                    profileRepository.SetAccount(profile.Id, accountId);
                }
                else
                {
                    // create profile
                    profileRepository.CreateProfile(firstName,
                                                    lastName,
                                                    email,
                                                    phone,
                                                    registration.CompanyId,
                                                    accountId);
                }


                //delete the registration so that it can't be used again.
                registrationRepository.DeleteAllByEmail(email);

                connectionManager.CommitTransaction();
            }
            catch (Exception)
            {
                connectionManager.Rollback();
                throw;
            }
        }
    }
}
