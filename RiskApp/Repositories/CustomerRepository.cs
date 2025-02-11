using RiskApp.Data;
using RiskApp.Models.Broker;
using RiskApp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using System.Dynamic;
using System.Data;
using System.Text.Json;

namespace RiskApp.Repositories
{
    public class CustomerRepository : RepositoryBase
    {
        public CustomerRepository(ConnectionManager connectionManager) : base(connectionManager)
        {

        }

        public Customer Create(Customer customer, Guid companyId, Guid createdById)
        {
            Guid newID = Guid.NewGuid();

            string SQL = @"INSERT INTO customer(
	customer_id, customer_name, firm_name, industry, industry_details, address, city, state, zip, email, primary_contact_name, business_phone, information, created_by_profile_id, company_id )
	VALUES (@ID, @CustomerName, @FirmName,  @Industry, @IndustryDetails, @Address, @City, @State, @Zip, @Email, @PrimaryContact, @Phone, @Information, @CreatedBy, @CompanyId );";

            Dictionary<string, object> queryParams = new Dictionary<string, object>()
            {
                {"ID", newID },
                {"CustomerName", DBSafeNull(customer.CustomerName)},
                {"FirmName", DBSafeNull(customer.FirmName)},
                {"Industry", DBSafeNull(customer.Industry)},
                {"IndustryDetails", DBSafeNull( customer.IndustryDetail) },
                {"Address", DBSafeNull(customer.Address) },
                {"City", DBSafeNull(customer.City)},
                {"State", DBSafeNull(customer.State)},
                {"Zip", DBSafeNull(customer.Zip)},
                {"Email", DBSafeNull(customer.Email)},
                {"PrimaryContact", DBSafeNull( customer.PrimaryContact )},
                {"Phone", DBSafeNull(customer.PrimaryContactPhone) },
                {"Information", DBSafeNull( customer.AdditionalInformation) },
                {"CreatedBy", createdById },
                {"CompanyId", companyId }
            };

            ExecuteNonQuery(SQL, queryParams);
            customer.Id = newID;
            return customer;
        }

        internal void Update(Customer customer, Guid customerId, Guid currentUserId)
        {
            string SQL = @"update customer
	                SET 
	                customer_name=@CustomerName, 
	                firm_name=@FirmName, 
	                primary_contact_name=@PrimaryContact, 
	                industry=@Industry, 
	                industry_details=@IndustryDetails, 
	                address=@Address, city=@City, 
	                state=@State, 
	                zip=@Zip, 
	                email=@Email, 
	                business_phone=@Phone, 
	                information=@Information
	                WHERE customer_id = @ID";

            Dictionary<string, object> queryParams = new Dictionary<string, object>()
            {
                {"ID", customerId },
                {"CustomerName", DBSafeNull(customer.CustomerName)},
                {"FirmName", DBSafeNull(customer.FirmName)},
                {"Industry", DBSafeNull(customer.Industry)},
                {"IndustryDetails", DBSafeNull(customer.IndustryDetail) },
                {"Address", DBSafeNull(customer.Address) },
                {"City", DBSafeNull(customer.City)},
                {"State", DBSafeNull(customer.State)},
                {"Zip", DBSafeNull(customer.Zip)},
                {"Email", DBSafeNull(customer.Email)},
                {"PrimaryContact", DBSafeNull(customer.PrimaryContact)},
                {"Phone", DBSafeNull(customer.PrimaryContactPhone) },
                {"Information", DBSafeNull(customer.AdditionalInformation) }
            };

            ExecuteNonQuery(SQL, queryParams);
        }

        public List<Customer> CompanyCustomers(Guid companyId)
        {
            List<Customer> accounts = new List<Customer>();
            string SQL = @"SELECT * from customer WHERE company_id = @CompanyId order by created_date";

            Dictionary<string, object> queryParams = new Dictionary<string, object>()
            {
                {"CompanyId", companyId }
            };
            using Npgsql.NpgsqlDataReader reader = ExecuteReader(SQL, queryParams);
            while (reader.Read())
            {
                Customer account = CustomerFromReader(reader);
                accounts.Add(account);
            }

            return accounts;
        }


        // deletes customer and all policies.
        public void Delete(Guid customerId)
        {
            string SQL = "DELETE from policy  WHERE customer_id = @ID  ";
            Dictionary<string, object> queryParams = new Dictionary<string, object>()
                {
                    {"ID", customerId }
                };

            ExecuteNonQuery(SQL, queryParams);

            SQL = @"DELETE from customer WHERE customer_id = @ID ";

            ExecuteNonQuery(SQL, queryParams);
        }

        public Customer CustomerById(Guid customerId)
        {

            string SQL = @"SELECT * from customer  WHERE customer_id = @ID ";

            Dictionary<string, object> queryParams = new Dictionary<string, object>()
            {
                {"ID", customerId }
            };
            using Npgsql.NpgsqlDataReader reader = ExecuteReader(SQL, queryParams);
            if (reader.Read())
            {
                Customer account = CustomerFromReader(reader);
                return account;
            }
            //nothing found
            return null;
        }

        private static Customer CustomerFromReader(NpgsqlDataReader reader)
        {
            return new Customer()
            {
                Id = reader.GetValue<Guid>("customer_id"),
                CustomerName = reader.GetValue<string>("customer_name"),
                FirmName = reader.GetValue<string>("firm_name"),
                Address = reader.GetValue<string>("address"),
                City = reader.GetValue<string>("city"),
                State = reader.GetValue<string>("state"),
                Zip = reader.GetValue<string>("zip"),
                Email = reader.GetValue<string>("email"),
                PrimaryContactPhone = reader.GetValue<string>("business_phone"),
                PrimaryContact = reader.GetValue<string>("primary_contact_name"),
                Industry = reader.GetValue<string>("industry"),
                IndustryDetail = reader.GetValue<string>("industry_details"),
                AdditionalInformation = reader.GetValue<string>("information"),
            };
        }


    }
}
