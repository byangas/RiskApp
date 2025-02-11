using RiskApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RiskApp.Extensions;
using RiskApp.Models;
using Npgsql;
using System.Text;

namespace RiskApp.Repositories
{
    public class CompanyRepository : RepositoryBase
    {
        public CompanyRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }


        public Company GetCompanyByEmailDomain(string emailDomain)
        {
            string SQL = @"Select * from company WHERE email_domain = @EmailDomain";

            var queryParams = new Dictionary<string, object>() { { "EmailDomain", emailDomain } };
            using Npgsql.NpgsqlDataReader reader = ExecuteReader(SQL, queryParams);
            if (reader.Read())
            {
                return CompanyFromReader(reader);
            }

            return null;
        }

        public IEnumerable<Company> GetCarriers(string search, bool appointedCarriers, Guid brokerageCompanyId)
        {

            List<Company> companies = new List<Company>();
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("Select company.* from company ");
            // if we are showing only appointed carriers
            if (appointedCarriers)
            {
                sqlBuilder.Append(" join carrier_appointment on carrier_appointment.carrier_company_id = company.company_id ");
            }

            //always show only carriers
            sqlBuilder.Append(" where company_type = 'carrier' ");
            ;
            Dictionary<string, object> queryParams = new Dictionary<string, object>();

            if (string.IsNullOrEmpty(search) == false)
            {
                // case insensitive search
                sqlBuilder.Append(" AND company_name ilike '%' || @Search || '%' ");
                queryParams.Add("Search", search);

            }

            if (appointedCarriers)
            {
                sqlBuilder.Append(" AND brokerage_company_id = @BrokerageCompanyId ");
                queryParams.Add("BrokerageCompanyId", brokerageCompanyId);
            }

            // always order results by name
            sqlBuilder.Append(" order by company_name");


            using Npgsql.NpgsqlDataReader reader = ExecuteReader(sqlBuilder.ToString(), queryParams);
            while (reader.Read())
            {
                companies.Add(CompanyFromReader(reader));
            }

            return companies;

        }

        public void CreateCarrierCompany(string companyName, string emailDomain)
        {
            string SQL = @"Insert INTO company  (email_domain, company_name, company_type)  
values(@EmailDomain, @CompanyName,'carrier') ";

            var queryParams = new Dictionary<string, object>() { 
                { "EmailDomain", emailDomain.ToLowerInvariant() },
                {"CompanyName", companyName }
            };
            ExecuteNonQuery(SQL, queryParams);
 
        }

        public Company GetCompanyById(Guid companyId)
        {

            string SQL = @"Select * from company WHERE company_id = @CompanyId";

            var queryParams = new Dictionary<string, object>() { { "CompanyId", companyId } };
            using Npgsql.NpgsqlDataReader reader = ExecuteReader(SQL, queryParams);
            if (reader.Read())
            {
                return CompanyFromReader(reader);
            }

            return null;
        }

        public static Company CompanyFromReader(NpgsqlDataReader reader)
        {

            return new Company()
            {
                Id = reader.GetValue<Guid>("company_id"),
                DomainName = reader.GetValue<string>("email_domain"),
                Name = reader.GetValue<string>("company_name"),
                CompanyType = reader.GetValue<CompanyType>("company_type"),
                Logo = reader.GetValue<string>("company_logo"),
            };
        }
    }
}
