using RiskApp.Data;
using RiskApp.Extensions;
using RiskApp.Models;
using RiskApp.Services;
using System;
using System.Collections.Generic;

namespace RiskApp.Repositories
{
    public class ProfileContactRepository : RepositoryBase
    {
        public ProfileContactRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

 

        public void CreateCompanyContact(Guid profileId, Guid currentCompanyId, Guid createdByProfileId)
        {
            var SQL = @"INSERT INTO profile_contact( profile_id, company_id, created_by) VALUES (@ProfileId, @CompanyId, @CreatedById)";

            Dictionary<string, object> queryParams = new Dictionary<string, object>() {
                { "ProfileId", profileId } ,
                { "CompanyId", currentCompanyId },
                { "CreatedById", createdByProfileId}
            };

            ExecuteNonQuery(SQL, queryParams);


        }

        public ProfileContact GetByProfileAndCompany(Guid companyId, Guid profileId)
        {
            string SQL = @"SELECT profile_contact_id, company_id, invalid_date, profile_id FROM profile_contact WHERE profile_id =@ProfileId AND company_id = @CompanyId;";


            Dictionary<string, object> queryParams = new Dictionary<string, object>() {
                { "ProfileId", profileId } ,
                { "CompanyId", companyId }
            };

            using var reader = ExecuteReader(SQL, queryParams);

            if (reader.Read())
            {
                var pc = new ProfileContact()
                {
                    Id = reader.GetValue<Guid>("profile_contact_id"),
                    CompanyId = reader.GetValue<Guid>("company_id"),
                    InvalidDate = reader.GetValue<DateTime?>("invalid_date"),
                    ProfileId = reader.GetValue<Guid>("profile_id")
                };

                return pc;
            }
            //not found
            return null;
        }

        public void ClearExpiration(Guid profileContactId)
        {
            var SQL = "Update profile_contact set invalid_date = null WHERE profile_contact_id = @ID";
            Dictionary<string, object> queryParams = new Dictionary<string, object>() {
                { "ID", profileContactId } 
            };

            ExecuteNonQuery(SQL, queryParams);
        }

        public void RemoveCompanyContact(Guid profileContactId)
        {
            var SQL = "Update profile_contact set invalid_date = CURRENT_TIMESTAMP WHERE profile_contact_id = @ID";
            Dictionary<string, object> queryParams = new Dictionary<string, object>() {
                { "ID", profileContactId }
            };

            ExecuteNonQuery(SQL, queryParams);
        }
    }
}
