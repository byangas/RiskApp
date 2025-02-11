using RiskApp.Models.Carrier;
using RiskApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Services
{

    public class CarrierAppointmentService
    {
        private readonly CarrierAppointmentRepository carrierAppointmentRepository;

        public CarrierAppointmentService(CarrierAppointmentRepository carrierAppointmentRepository)
        {
            this.carrierAppointmentRepository = carrierAppointmentRepository;
        }
        public CarrierAppointment GetAppointmentForBroker(Guid carrierCompanyId, Guid currentUserCompanyId)
        {
            return carrierAppointmentRepository.GetAppointmentForBroker(carrierCompanyId, currentUserCompanyId);
        }

        public CarrierAppointment AddAppointmentForBroker(Guid carrierCompanyId, Guid currentUserCompanyId, Guid currentUserProfileId)
        {
            return carrierAppointmentRepository.AddAppointmentForBroker(carrierCompanyId, currentUserCompanyId, currentUserProfileId);
        }

        public void UpdateCarrierAppointment(Guid carrierCompanyId, Guid brokerageCompanyId, string brokerNotes)
        {
             carrierAppointmentRepository.UpdateCarrierAppointment(carrierCompanyId, brokerageCompanyId, brokerNotes);
        }

        public void RemoveCarrierAppointment(Guid carrierCompanyId, Guid brokerageCompanyId)
        {
            carrierAppointmentRepository.RemoveCarrierAppointment(carrierCompanyId, brokerageCompanyId);
        }
    }
}
