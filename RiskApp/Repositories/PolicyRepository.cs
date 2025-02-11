using Npgsql;
using NpgsqlTypes;
using RiskApp.Data;
using RiskApp.Extensions;
using RiskApp.Models.Broker;
using RiskApp.Models.Broker.Policy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Text.Json;

namespace RiskApp.Repositories
{
    public class PolicyRepository : RepositoryBase
    {
        public PolicyRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public IEnumerable<PolicySummary> GetCompanyPolicies(Guid companyId, string orderBy)
        {
            List<PolicySummary> policies = new List<PolicySummary>();
            string SQL = @"
SELECT POLICY_ID,
	POLICY.CUSTOMER_ID,
	STATUS,
	DESCRIPTION,
	FIRST_NAME,
	LAST_NAME,
	COALESCE(CUSTOMER_NAME, FIRM_NAME) AS CUSTOMER,
	(SELECT COUNT(*) AS marketing_sheet_count FROM MARKETING_SHEET_CONTACT WHERE POLICY.POLICY_ID = MARKETING_SHEET_CONTACT.POLICY_ID) , 
	POLICY.CREATED_DATE,
	renewal_date
FROM POLICY
JOIN PROFILE AS CREATED_PROFILE ON CREATED_BY_PROFILE_ID = CREATED_PROFILE.PROFILE_ID
JOIN CUSTOMER ON POLICY.CUSTOMER_ID = CUSTOMER.CUSTOMER_ID
WHERE CUSTOMER.company_id = @CompanyId and status='open' ";
            if(orderBy == "renewal")
            {
                SQL += " ORDER BY POLICY.renewal_date ASC";
            }
            else
            {
                SQL += " ORDER BY POLICY.CREATED_DATE ASC";
            }

            NpgsqlDataReader reader = ExecuteReader(SQL, "CompanyId", companyId);

            while (reader.Read())
            {
                PolicySummary summary = new PolicySummary();
                summary.BrokerName = string.Format("{0} {1}", reader.GetValue<string>("first_name"), reader.GetValue<string>("last_name"));
                summary.CustomerId = reader.GetValue<Guid>("customer_id");
                summary.PolicyId = reader.GetValue<Guid>("policy_id");
                summary.Status = reader.GetValue<string>("status");
                summary.RenewalDate = reader.GetValue<DateTime?>("renewal_date");
                if(summary.RenewalDate.HasValue)
                {
                    //because renewal date is just stored as a date in DB. it has no "timezone" per se.
                    summary.RenewalDate = summary.RenewalDate.Value.ToUniversalTime();
                }

                summary.CreatedDate = reader.GetValue<DateTime>("created_date");
                summary.Description = reader.GetValue<string>("description");
                summary.Customer = reader.GetValue<string>("customer");
                summary.MarketingSheetCount = reader.GetValue<Int64>("marketing_sheet_count");
                policies.Add(summary);
            }

            return policies;
        }

        public IEnumerable<PolicyNote> GetPolicyNotes(Guid policyId)
        {
            List<PolicyNote> notes = new List<PolicyNote>();

            string SQL = @"select policy_note.*, first_name, last_name, profile from policy_note
               join profile on profile.profile_id = policy_note.created_by where policy_id = @PolicyId 
order by created_date ASC";
            NpgsqlDataReader reader = ExecuteReader(SQL, "PolicyId", policyId);
            while (reader.Read())
            {
                var note = new PolicyNote();
                note.Id = reader.GetValue<Guid>("policy_note_id");
                note.CreatedBy = string.Format("{0} {1}", reader.GetValue<string>("first_name"), reader.GetValue<string>("last_name"));
                note.Note = reader.GetValue<string>("note");
                note.Created = reader.GetValue<DateTime>("created_date");
                notes.Add(note);
            }
            return notes;
        }

        public void CreatePolicyNote(Guid policyId, string note, Guid createdBy)
        {
            string SQL = @"INSERT INTO policy_note (policy_id, note, created_by) values
            (@PolicyId, @Note, @CreatedBy)";
            Dictionary<string, object> queryParams = new Dictionary<string, object>()
            {
                {"PolicyId", policyId },
                {"Note", note },
                {"CreatedBy", createdBy }
            };
            ExecuteNonQuery(SQL, queryParams);
        }

        public void UpdatePolicyDetails(Guid policyId, string description, DateTime? renewalDate)
        {
            string SQL = @"update policy set description = @Description, renewal_date = @RenewalDate WHERE policy_id = @PolicyId";
            Dictionary<string, object> queryParams = new Dictionary<string, object>()
            {
                {"Description", description },
                { "RenewalDate", DBSafeNull(renewalDate) },
                {"PolicyId", policyId }
            };
            ExecuteNonQuery(SQL, queryParams);
         
        }

        public void DeletePolicy(Guid policyId)
        {
            string SQL = "delete from policy  Where policy_id = @PolicyId;";
            ExecuteNonQuery(SQL, "PolicyId", policyId);
        }

        public void DeleteMarketingSheetContactsForPolicy(Guid policyId)
        {
            string SQL = "delete from marketing_sheet_contact  Where policy_id = @PolicyId;";
            ExecuteNonQuery(SQL, "PolicyId", policyId);
        }

        public void DeleteMarketingSheetNotesForPolicy(Guid policyId)
        {
            string SQL = @" delete from marketing_sheet_contact_note where marketing_sheet_contact_note_id in (
Select marketing_sheet_contact_note_id from marketing_sheet_contact
join marketing_sheet_contact_note
on marketing_sheet_contact_note.marketing_sheet_contact_id =
    marketing_sheet_contact.marketing_sheet_contact_id
Where policy_id = @PolicyId)";

            ExecuteNonQuery(SQL, "PolicyId", policyId);
            
        }

        public Guid CreatePolicy(Guid customerId, PolicyEdit policy, Guid createdByProfileId)
        {
            string SQL = @" INSERT INTO policy (customer_id, status, description, renewal_date,   insurance_types, detail, created_by_profile_id)
                            VALUES(@CustomerId, 'open', @Description, @RenewalDate,  @InsuranceTypes, @Detail, @CreatedBy) returning policy_id";

            using var command = connectionManager.GetCommand(SQL);
            command.Parameters.AddWithValue("CreatedBy", createdByProfileId);
            command.Parameters.AddWithValue("RenewalDate", DBSafeNull(policy.RenewalDate));
            command.Parameters.AddWithValue("Description", policy.Description);
            command.Parameters.AddWithValue("CustomerId", customerId);
            command.Parameters.AddWithValue("InsuranceTypes", NpgsqlDbType.Array | NpgsqlDbType.Varchar, policy.InsuranceTypes);
            command.Parameters.AddWithValue("Detail", NpgsqlDbType.Jsonb, DBSafeNull(policy.Detail));

            Guid id = (Guid)command.ExecuteScalar();
            connectionManager.CloseConnection();
            return id;
        }

        public dynamic GetPolicy(Guid policyId)
        {
            string SQL = @"select policy_id, description, renewal_date, detail, created_date, customer_id, status, created_by_profile_id, insurance_types from  policy 
WHERE policy_id = @PolicyId
Order by created_date DESC ";

            using var reader = ExecuteReader(SQL, new Dictionary<string, object> { { "PolicyId", policyId } });
            if (reader.Read())
            {
                dynamic policy = PolicyFromReader(reader);
                return policy;
            }
            else
            {
                return null;
            }

        }

        public IEnumerable<dynamic> GetPolicies(Guid customerId)
        {
            var policies = new List<dynamic>();
            string SQL = @"select policy_id, detail, description, renewal_date, created_date, customer_id, status, created_by_profile_id, insurance_types from  policy 
WHERE customer_id = @CustomerId
Order by created_date DESC ";

            using var reader = ExecuteReader(SQL, new Dictionary<string, object> { { "CustomerId", customerId } });
            while (reader.Read())
            {
                dynamic policy = PolicyFromReader(reader);
                policies.Add(policy);
            }
            return policies;
        }

        public void UpdateAppetite(Guid policyId, string policy, string[] insuranceTypes)
        {
            if(insuranceTypes == null)
            {
                insuranceTypes = new string[0];
            }

            string SQL = @"UPDATE policy SET insurance_types=@InsuranceTypes, detail=@Detail WHERE policy_id = @PolicyId ";

            var command = connectionManager.GetCommand(SQL);

            command.Parameters.AddWithValue("PolicyId", policyId);
            command.Parameters.AddWithValue("InsuranceTypes", NpgsqlDbType.Array | NpgsqlDbType.Varchar, insuranceTypes);
            command.Parameters.AddWithValue("Detail", NpgsqlDbType.Jsonb, DBSafeNull(policy));
            command.ExecuteNonQuery();
        }

        private static dynamic PolicyFromReader(NpgsqlDataReader reader)
        {
            dynamic policy = new ExpandoObject();
            policy.id = reader.GetValue<Guid>("policy_id");
            policy.customerId = reader.GetValue<Guid>("customer_id");
            policy.description = reader.GetValue<string>("description");
            var detail = reader.GetValue<string>("detail");
            if (detail != null)
            {
                policy.detail = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(detail);
            }

            policy.insuranceTypes = reader.GetValue<string[]>("insurance_types");
            //because date gets created in the timezone of server (.net behaviour), we convert back to UTC
            policy.createdDate = reader.GetDateTime("created_date").ToUniversalTime();
            DateTime? renewalDate = reader.GetValue<DateTime?>("renewal_date");
            if(renewalDate != null)
            {
                policy.renewalDate = renewalDate.Value.ToUniversalTime();
            }
            policy.status = reader.GetString("status");
            policy.createByProfileId = reader.GetValue<Guid>("created_by_profile_id");
            return policy;
        }
        public Guid? CreateMarketingSheetContact(Guid policyId, Guid contactProfileId, Guid createdByProfileId)
        {
            string SQL = @"INSERT INTO public.marketing_sheet_contact(
	policy_id, profile_id, created_by_profile_id)
	VALUES (@PolicyId, @ContactProfileId, @CreatedByProfileId ) ON CONFLICT DO NOTHING returning marketing_sheet_contact_id ;";

            Guid? newId = ExecuteScalar<Guid?>(SQL, new Dictionary<string, object>()
            {
                {"PolicyId", policyId },
                {"ContactProfileId", contactProfileId },
                {"CreatedByProfileId", createdByProfileId }
            });

            return newId;
        }

        public IEnumerable<MarketingSheetContact> GetMarketingSheet(Guid policyId)
        {
            List<MarketingSheetContact> marketingSheet = new List<MarketingSheetContact>();
            string SQL = @"select policy_id, marketing_sheet_contact_id, premium, commission,  status,company_name,company_logo,
profile.profile_id, first_name, last_name, email, created_by_profile_id, phone, mobile_phone from marketing_sheet_contact 
join profile on profile.profile_id = marketing_sheet_contact.profile_id
join company on profile.company_id = company.company_id WHERE policy_id = @PolicyID ORDER BY marketing_sheet_contact.created_date";

            using Npgsql.NpgsqlDataReader reader = ExecuteReader(SQL, new Dictionary<string, object>()
            {
                {"PolicyId", policyId }
            });

            while (reader.Read())
            {
                MarketingSheetContact marketingSheetContact = MarketingSheetContactFromReader(reader);
                marketingSheet.Add(marketingSheetContact);
            }
            return marketingSheet;

        }

        public MarketingSheetContact GetMarketingSheetContact(Guid marketingSheetContactId)
        {
            string SQL = @"select  policy_id, marketing_sheet_contact_id, premium, commission,  status,company_name,company_logo,
profile.profile_id, first_name, last_name, email, created_by_profile_id, mobile_phone, phone from marketing_sheet_contact 
join profile on profile.profile_id = marketing_sheet_contact.profile_id
join company on profile.company_id = company.company_id WHERE marketing_sheet_contact_id = @MarketingSheetContactId";

            using Npgsql.NpgsqlDataReader reader = ExecuteReader(SQL, new Dictionary<string, object>()
            {
                {"MarketingSheetContactId", marketingSheetContactId }
            });

            if (reader.Read())
            {
                return MarketingSheetContactFromReader(reader);
            }
            //not found
            return null;

        }

        private static MarketingSheetContact MarketingSheetContactFromReader(NpgsqlDataReader reader)
        {
            return new MarketingSheetContact()
            {
                MobilePhone = reader.GetValue<string>("mobile_phone"),
                Phone = reader.GetValue<string>("phone"),
                Email = reader.GetValue<string>("email"),
                Logo = reader.GetValue<string>("company_logo"),
                CompanyName = reader.GetValue<string>("company_name"),
                Name = $"{reader.GetValue<string>("first_name")} {reader.GetValue<string>("last_name")}",
                Id = reader.GetValue<Guid>("marketing_sheet_contact_id"),
                Status = reader.GetValue<MarketingSheetContactStatus>("status"),
                Commission = reader.GetValue<int>("commission"),
                Premium = reader.GetValue<int>("premium"),
                ProfileId = reader.GetValue<Guid>("profile_id"),
                PolicyId = reader.GetValue<Guid>("policy_id"),
                CreatedByProfileId = reader.GetValue<Guid>("created_by_profile_id")
            };
        }

        public void AddMarketingSheetContactNote(Guid marketingSheetContactId, string note)
        {
            ExecuteNonQuery("insert into marketing_sheet_contact_note (marketing_sheet_contact_id, note)" +
                " values(@MarketingSheetContactId, @Note) ",
                new Dictionary<string, object>(){
                    {"Note", note},
                    { "MarketingSheetContactId", marketingSheetContactId}
                });
        }

        public IEnumerable<MarketingSheetContactNote> MarketingSheetContactNotes(Guid marketingSheetContactId)
        {
            List<MarketingSheetContactNote> notes = new List<MarketingSheetContactNote>();
            using Npgsql.NpgsqlDataReader reader = ExecuteReader("Select * from  marketing_sheet_contact_note WHERE marketing_sheet_contact_id = @MarketingSheetContactId ORDER BY created_date ",
                new Dictionary<string, object>(){
                    { "MarketingSheetContactId", marketingSheetContactId}
                });
            while (reader.Read())
            {
                notes.Add(new MarketingSheetContactNote()
                {
                    Created = reader.GetValue<DateTime>("created_date").ToUniversalTime(),
                    Id = reader.GetValue<Guid>("marketing_sheet_contact_note_id"),
                    Note = reader.GetValue<string>("note")
                });
            }

            return notes;
        }



        public void UpdateMarketingSheetField(Guid marketingSheetContactId, string field, object value)
        {

            ExecuteNonQuery($"update marketing_sheet_contact set {field}=@{field}  WHERE marketing_sheet_contact_id=@MarketingSheetContactId", new Dictionary<string, object>()
            {
                {field, value},
                { "MarketingSheetContactId", marketingSheetContactId}
            });
        }


        public void DeleteMarketingSheetContact(Guid marketingSheetContactId)
        {
            ExecuteNonQuery("Delete from marketing_sheet_contact where marketing_sheet_contact_id = @MarketingSheetContactId",
                new Dictionary<string, object>() { { "MarketingSheetContactId", marketingSheetContactId } });
        }
    }
}