using RiskApp.Models.User;
using RiskApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Services
{
    public class ProfileService
    {
        private readonly ProfileRepository profileRepository;
        private readonly CompanyRepository companyRepository;
        private readonly UserAccountRepository accountRepository;

        public ProfileService(ProfileRepository profileRepository, CompanyRepository companyRepository, UserAccountRepository accountRepository)
        {
            this.profileRepository = profileRepository;
            this.companyRepository = companyRepository;
            this.accountRepository = accountRepository;
        }

        // UserInfo is a light-weight object that is used by the client to create the 'context' of the logged in user
        public UserInfo GetUserInfo(Guid userProfileId)
        {
            //TODO: optimize
            UserInfo userInfo = profileRepository.GetUserInfo(userProfileId);
            if (userInfo == null)
                return null;
            userInfo.Company = companyRepository.GetCompanyById(userInfo.CompanyId);
            if (userInfo.AccountId.HasValue)
            {
                userInfo.Roles = accountRepository.GetApplicationRoles(userInfo.AccountId.Value);
            }

            return userInfo;
        }

        public Profile GetProfile(Guid profileId)
        {
            return profileRepository.GetProfile(profileId);
        }

        /// <summary>
        /// Updates the profile based on the Model passed in. updates based on the 
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="profile"></param>
        public void UpdateProfile(Profile profile)
        {
            profileRepository.UpdateProfile(profile);
        }
    }
}
