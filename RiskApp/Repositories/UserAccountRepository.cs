using Npgsql;
using RiskApp.Data;
using RiskApp.Models.User;
using System;
using System.Collections.Generic;
namespace RiskApp.Repositories
{
    public class UserAccountRepository : RepositoryBase
    {
        public UserAccountRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public SignInInfo GetSignInInfo(string email)
        {

            string SQL = @"SELECT password, account.account_id, profile_id, company_id from account
JOIN profile on profile.account_id = account.account_id
WHERE email = @email";

            using var reader = ExecuteReader(SQL, queryparams: new Dictionary<string, object>(){ {"email", email}});

            if (reader.HasRows == false)
            {
                return null;
            }

            reader.Read();

            var signinInfo = new SignInInfo
            {
                AccountId = GetValue<Guid>(reader, "account_id"),
                ProfileId = GetValue<Guid>(reader, "profile_id"),
                Password = GetValue<string>(reader, "password"),
                CompanyId = GetValue<Guid?>(reader, "company_id")
            };
            return signinInfo;
        }

        public string[] GetApplicationRoles(Guid accountId)
        {
            List<string> roles = new List<string>();

            string SQL = @"Select name from account_role  join application_role on application_role.application_role_id = account_role.application_role_id  WHERE account_id = @AccountId; ";

            using var reader = ExecuteReader(
                SQL,
                queryparams: new Dictionary<string, object>()
                {
                    { "AccountId", accountId }
                }
            );

            while (reader.Read())
            {
                roles.Add(GetValue<string>(reader, "name"));
            }
            return roles.ToArray();
        }

        public void CreateAccount(Guid accountId, string hashedPassword)
        {
            string SQL = @"INSERT INTO account(account_id, password) VALUES(@AccountId, @Password); ";
            var queryParams = new Dictionary<string, object>()
            {
                { "AccountId", accountId },
                { "Password", hashedPassword }
            };
            ExecuteNonQuery(SQL, queryParams);
        }

        public void CreateRoles(Guid accountId, Guid[] roles)
        {
            if(roles == null)
            {
                return;
            }
            foreach (var role in roles)
            {
                string SQL = @"INSERT INTO account_role(
                            account_id, application_role_id)
	                        VALUES(@AccountId, @ApplicationRoleId); ";
                var queryParams = new Dictionary<string, object>()
                {
                    { "AccountId", accountId },
                    {  "ApplicationRoleId", role  }
                };
                ExecuteNonQuery(SQL, queryParams);
            }
        }
    }
}
