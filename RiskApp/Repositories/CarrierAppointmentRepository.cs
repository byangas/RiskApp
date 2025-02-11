using RiskApp.Data;
using RiskApp.Models.Carrier;
using RiskApp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Repositories
{
    public class CarrierAppointmentRepository : RepositoryBase
    {
        public CarrierAppointmentRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public CarrierAppointment GetAppointmentForBroker(Guid carrierCompanyId, Guid brokerCompanyId)
        {
            string SQL = @"select * from carrier_appointment
join profile on carrier_appointment.created_by = profile.profile_id
where brokerage_company_id = @BrokerCompanyId AND carrier_company_id = @CarrierCompanyId";

            var queryParams = new Dictionary<string, object>()
            {
                {"BrokerCompanyId", brokerCompanyId },
                { "CarrierCompanyId", carrierCompanyId }
            };

            using Npgsql.NpgsqlDataReader reader = ExecuteReader(SQL, queryParams);

            if (reader.Read())
            {
    
                var appointment = new CarrierAppointment()
                {
                    Id = reader.GetValue<Guid>("carrier_appointment_id"),
                    BrokerNotes = reader.GetValue<string>("brokerage_notes"),
                    CreatedByName = $"{reader.GetValue<string>("first_name")} {reader.GetValue<string>("last_name")}",
                    CreatedDate = reader.GetValue<DateTime>("created_date"),
                    CreateByProfileId = reader.GetValue<Guid>("created_by")
                };
                return appointment;
            }
            //not found
            return null;
        }

        public void RemoveCarrierAppointment(Guid carrierCompanyId, Guid brokerageCompanyId)
        {
            string SQL = @"delete from carrier_appointment where brokerage_company_id = @BrokerCompanyId AND carrier_company_id = @CarrierCompanyId";

            var queryParams = new Dictionary<string, object>()
            {
                {"BrokerCompanyId", brokerageCompanyId },
                { "CarrierCompanyId", carrierCompanyId }
            };

            ExecuteNonQuery(SQL, queryParams);
        }

        internal void UpdateCarrierAppointment(Guid carrierCompanyId, Guid brokerageCompanyId, string brokerNotes)
        {
            string SQL = @"update carrier_appointment set brokerage_notes = @BrokerNotes
where brokerage_company_id = @BrokerCompanyId AND carrier_company_id = @CarrierCompanyId";

            var queryParams = new Dictionary<string, object>()
            {
                {"BrokerCompanyId", brokerageCompanyId },
                {"CarrierCompanyId", carrierCompanyId },
                {"BrokerNotes", brokerNotes }
            };

            ExecuteNonQuery(SQL, queryParams);
        }

        internal CarrierAppointment AddAppointmentForBroker(Guid carrierCompanyId, Guid brokerageCompanyId, Guid createdByProfileId)
        {
            // we have "ON CONFLICT DO NOTHING" here intentionally so that we can use it to create relationship if needed, but if
            // it's already there, we do nothing. Some would call it an "Upsert" thought I don't like that expression.
            string SQL = @"insert into carrier_appointment (carrier_company_id, brokerage_company_id, created_by) VALUES ( @CarrierCompanyId, @BrokerageId, @CreatedBy) ON CONFLICT DO NOTHING";
            var queryParams = new Dictionary<string, object>()
            {
                { "BrokerageId", brokerageCompanyId },
                { "CarrierCompanyId", carrierCompanyId },
                { "CreatedBy", createdByProfileId }
            };

            ExecuteNonQuery(SQL, queryParams);
            return GetAppointmentForBroker(carrierCompanyId, brokerageCompanyId);
        }
    }
}
