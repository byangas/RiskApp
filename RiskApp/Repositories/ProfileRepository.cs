using Npgsql;
using RiskApp.Data;
using RiskApp.Models.User;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using RiskApp.Extensions;
using System.Diagnostics;

namespace RiskApp.Repositories
{
    public class ProfileRepository : RepositoryBase
    {
        public ProfileRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public void CreateProfile(ProfileCreate profileCreate)
        {
            string SQL = @"INSERT INTO profile ( company_id, first_name, last_name, phone, email, title, specialty, mobile_phone )
    VALUES( @CompanyId, @FirstName, @LastName, @Phone, @Email, @Title, @Specialty, @MobilePhone); ";

            ExecuteNonQuery(SQL, new Dictionary<string, object> {
                { "Title", profileCreate.Title },
                { "CompanyId", profileCreate.CompanyId },
                { "FirstName", profileCreate.FirstName },
                { "LastName", profileCreate.LastName },
                { "Email", profileCreate.Email },
                { "Phone", DBSafeNull(profileCreate.Phone) },
                { "Specialty", DBSafeNull(profileCreate.Specialty) },
                { "MobilePhone" , DBSafeNull(profileCreate.MobilePhone) }
            });
        }

        public void CreateProfile(string firstName,
                                    string lastName,
                                    string email,
                                    string phone,
                                    Guid companyId,
                                    Guid? accountId = null)
        {
            string SQL = @"INSERT INTO profile ( account_id ,  company_id ,  first_name ,  last_name ,  phone ,  email )
    VALUES(@AccountId, @CompanyId, @FirstName, @LastName, @Phone, @Email); ";

            ExecuteNonQuery(SQL, new Dictionary<string, object> {
                { "AccountId", DBSafeNull(accountId) },
                { "CompanyId", DBSafeNull(companyId) },
                { "FirstName", firstName },
                { "LastName", lastName },
                { "Email", email },
                { "Phone", phone }
            });
        }

        public void UpdateProfile(Profile profile)
        {
            string SQL = " UPDATE profile SET first_name=@FirstName, last_name=@LastName, phone=@Phone, title=@Title, specialty=@Specialty, " +
                " mobile_phone=@MobilePhone WHERE profile_id = @ProfileId;";
            Dictionary<string, object> queryParams = new Dictionary<string, object>()
            {
                {"ProfileId", profile.Id },
                {"FirstName", DBSafeNull( profile.FirstName) },
                {"LastName", DBSafeNull(profile.LastName) },
                {"Phone", DBSafeNull(profile.Phone) },
                {"Specialty", DBSafeNull( profile.Specialty) },
                {"Title", DBSafeNull(profile.Title) },
                {"MobilePhone", DBSafeNull(profile.MobilePhone) }

            };
            int recordsAffected = ExecuteNonQuery(SQL, queryParams);
            Debug.Assert(recordsAffected > 0, "Profile Update failed");
        }

        public Profile GetProfile(Guid profileId)
        {
            string SQL = " SELECT * From Profile where profile_id = @ProfileId";
            using NpgsqlDataReader reader = ExecuteReader(SQL, "ProfileId", profileId);
            if (reader.Read())
            {
                return ProfileFromReader(reader); ;
            }
            // not found
            return null;
        }

        public UserInfo GetUserInfo(Guid userProfileId)
        {
            string SQL = @"SELECT *  from profile WHERE  profile_id  = @ProfileId; ";
            var queryParams = new Dictionary<string, object>()
            {
                { "ProfileId", userProfileId }
            };

            using NpgsqlDataReader reader = ExecuteReader(SQL, queryParams);
            if (!reader.Read())//expecting only one row
                return null;

            UserInfo userInfo = new UserInfo()
            {
                Email = reader.GetValue<string>("email"),
                Id = userProfileId,
                Name = string.Format("{0} {1}", reader.GetValue<string>("first_name"), reader.GetValue<string>("last_name")),
                Phone = reader.GetValue<string>("phone"),
                MobilePhone = reader.GetValue<string>("mobile_phone"),
                AccountId = reader.GetValue<Guid?>("account_id"),
                CompanyId = reader.GetValue<Guid>("company_id"),
            };
            return userInfo;

        }

        public Profile GetProfile(string email)
        {
            string SQL = " SELECT * From Profile where email = @Email";
            using NpgsqlDataReader reader = ExecuteReader(SQL, "Email", email);
            if (reader.Read())
            {
                return ProfileFromReader(reader); ;
            }
            // not found
            return null;
        }

        public static Profile ProfileFromReader(NpgsqlDataReader reader)
        {
            Profile profile = new Profile();
            ProfileFromReader(reader, profile);
            return profile;
        }

        public void SetAccount(Guid profileId, Guid accountId)
        {
            string SQL = "UPDATE profile SET account_id = @AccountId WHERE profile_id = @ProfileId ";
            ExecuteNonQuery(SQL, new Dictionary<string, object>()
            {
                { "ProfileId", profileId },
                { "AccountId", accountId }
            });
        }

        public static void ProfileFromReader(NpgsqlDataReader reader, Profile source)
        {
            source.Id = reader.GetValue<Guid>("profile_id");
            source.Email = reader.GetValue<string>("email");
            source.FirstName = reader.GetValue<string>("first_name");
            source.LastName = reader.GetValue<string>("last_name");
            source.Name = string.Format("{0} {1}", reader.GetValue<string>("first_name"), reader.GetValue<string>("last_name"));
            source.Phone = reader.GetValue<string>("phone");
            source.MobilePhone = reader.GetValue<string>("mobile_phone");
            source.AccountId = reader.GetValue<Guid?>("account_id");
            source.CompanyId = reader.GetValue<Guid>("company_id");
            source.Title = reader.GetValue<string>("title");
            source.Specialty = reader.GetValue<string>("specialty");
            
           
        }
    }
}
