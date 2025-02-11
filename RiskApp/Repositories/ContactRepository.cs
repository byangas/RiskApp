using Npgsql;
using RiskApp.Data;
using RiskApp.Models;
using RiskApp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Repositories
{
    // this is more like a "business domain" object. It views "contacts" as 
    // amalgamation of profiles, profile contacts, company, etc. NOT a one-to-one
    public class ContactRepository : RepositoryBase
    {
        public ContactRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public Contact GetContact(Guid currentCompanyId, string email = null, Guid? profileId = null)
        {
            Dictionary<string, object> queryParams = new Dictionary<string, object>() {
                { "CompanyId", currentCompanyId }
            };

            string whereClause = "";

            if (!string.IsNullOrEmpty(email))
            {
                whereClause = " WHERE profile.email = @Email ";
                queryParams.Add("Email", email);
            }
            else if (profileId.HasValue)
            {
                whereClause = " WHERE profile.profile_id = @ProfileID ";
                queryParams.Add("ProfileID", profileId);
            }

            string SQL = @" Select profile_contact_id, company.company_name, company.company_type, company.email_domain, company.company_website, company.company_logo, profile.* from profile 
JOIN company on profile.company_id = company.company_id AND company.company_type = 'carrier'
LEFT JOIN profile_contact 
	on profile_contact.profile_id = profile.profile_id 
    AND profile_contact.invalid_date is null
	AND profile_contact.company_id = @CompanyId " + whereClause;

            using NpgsqlDataReader reader = ExecuteReader(SQL, queryParams);
            if (reader.Read())
            {
                Contact contact = ContactFromReader(reader);
                return contact;
            }
            return null;
        }

        public IEnumerable<Contact> BrokerCompanyContacts(Guid currentCompanyId, bool showOnlyMyCompany, string search, string sortBy, Guid? filterByCompanyId)
        {
            List<Contact> contacts = new List<Contact>();

            Dictionary<string, object> queryParams = new Dictionary<string, object>() {
                { "CompanyId", currentCompanyId }
            };

            string whereClause = "";
            if (showOnlyMyCompany)
            {
                whereClause = " WHERE profile_contact_id is not null ";
            }

            if(filterByCompanyId.HasValue)
            {
                whereClause += string.IsNullOrWhiteSpace(whereClause) ? " WHERE " : " AND ";
                whereClause += " profile.company_id = @FilterByCompanyId ";
                queryParams.Add("FilterByCompanyId", filterByCompanyId.Value);
            }


            if (!string.IsNullOrWhiteSpace(search))
            {
                whereClause += string.IsNullOrWhiteSpace(whereClause) ? " WHERE " : " AND ";
                whereClause += @"(first_name ilike '%' || @Search || '%' 
OR last_name ilike '%' || @Search || '%' 
OR title  ilike '%' || @Search || '%' 
OR specialty ilike '%' || @Search || '%'
OR company.company_name  ilike '%' || @Search || '%')";
                queryParams.Add("Search", search);
            }


            string SQL = @"Select profile_contact_id, company.company_name, company.company_type, company.email_domain, company.company_website, company.company_logo, profile.* from profile 
JOIN company on profile.company_id = company.company_id AND company.company_type = 'carrier'
LEFT JOIN profile_contact 
	on profile_contact.profile_id = profile.profile_id 
    AND profile_contact.invalid_date is null
	AND profile_contact.company_id = @CompanyId " + whereClause;

            //determine sort
            string orderByClause = " ORDER BY company.company_name asc"; // this is default sort

            switch (sortBy)
            {
                case "company_desc":
                    orderByClause = " ORDER BY company.company_name desc";
                    break;
                case "name_asc":
                    orderByClause = " ORDER BY profile.last_name asc";
                    break;
                case "name_desc":
                    orderByClause = " ORDER BY profile.last_name desc";
                    break;
            }

            SQL += orderByClause;




            using Npgsql.NpgsqlDataReader reader = ExecuteReader(SQL, queryParams);
            while (reader.Read())
            {
                contacts.Add(ContactFromReader(reader));
            }

            return contacts;

        }

        private Contact ContactFromReader(NpgsqlDataReader reader)
        {
            var contact = new Contact();
            ProfileRepository.ProfileFromReader(reader, contact);
            contact.Company = CompanyRepository.CompanyFromReader(reader);
            Guid? temp = reader.GetValue<Guid?>("profile_contact_id");
            contact.CompanyContact = temp.HasValue;
            return contact;
        }
    }
}
