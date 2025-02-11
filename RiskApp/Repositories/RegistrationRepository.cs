using Npgsql;
using RiskApp.Data;
using RiskApp.Extensions;
using RiskApp.Models.User;
using System;
using System.Collections.Generic;


namespace RiskApp.Repositories
{
    public class RegistrationRepository : RepositoryBase
    {
        public RegistrationRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }


        public void CreateRegistration(string email, string registrationCode, DateTime expiresAt, Guid companyId, Guid[] roles)
        {

            string SQL = @"INSERT INTO registration(
                        registration_code, 
                        expires_date,
                        email, 
                        company_id,
                        roles
                        )
	                    VALUES( @RegistrationCode,@ExpiresAt, @Email, @CompanyId, @Roles ); ";
            Dictionary<string, object> @params = new Dictionary<string, object>()
            {
                { "RegistrationCode", registrationCode},
                { "ExpiresAt", expiresAt },
                {  "Email", email },
                { "CompanyId", companyId },
                { "Roles", roles }
            };

            ExecuteNonQuery(SQL, @params);
        }


        public void DeleteAllByEmail(string email)
        {
            string SQL = "Delete from registration where email = @Email";
            ExecuteNonQuery(SQL, new Dictionary<string, object>() { { "Email", email } });

        }

        public Registration GetValidRegistration(string email, string registrationCode)
        {
            string SQL = @"Select * from registration where email = @Email AND registration_code = @RegistrationCode AND expires_date > CURRENT_TIMESTAMP";
            Dictionary<string, object> @params = new Dictionary<string, object>()
            {
                { "RegistrationCode", registrationCode},
                {  "Email", email }
            };
            using NpgsqlDataReader reader = ExecuteReader(SQL, @params);
            if(reader.Read())
            {
                return new Registration()
                {
                    CompanyId = reader.GetValue<Guid>("company_id"),
                    Email = email,
                    Id = reader.GetValue<Guid>("registration_id"),
                    RegistrationCode = registrationCode,
                    Roles = reader.GetValue<Guid[]>("roles"),
                };
            }
            return null;

        }
    }
}