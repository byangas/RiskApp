using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RiskApp.Models.Carrier;
using RiskApp.Services;
using RiskApp.Extensions;

namespace RiskApp.Controllers.API
{
    [Route("api/carrier")]
    [ApiController]
    public class CarrierAppointmentController : ControllerBase
    {
        private readonly CarrierAppointmentService carrierAppointmentService;

        public CarrierAppointmentController(CarrierAppointmentService carrierAppointmentService)
        {
            this.carrierAppointmentService = carrierAppointmentService;
        }

        [HttpGet("{carrierCompanyId}/appointment")]
        public ActionResult<CarrierAppointment> GetAppointment([FromRoute] Guid carrierCompanyId)
        {
            Guid currentUserCompanyId = User.CurrentUserCompanyId();
            CarrierAppointment appointment = carrierAppointmentService.GetAppointmentForBroker(carrierCompanyId, currentUserCompanyId);
            return Ok(appointment);
        }

        [HttpPost("{carrierCompanyId}/appointment")]
        public ActionResult<CarrierAppointment> AddAppointment([FromRoute] Guid carrierCompanyId)
        {
            Guid currentUserCompanyId = User.CurrentUserCompanyId();
            Guid currentUserProfileId = User.CurrentUserId();
            CarrierAppointment appointment = carrierAppointmentService.AddAppointmentForBroker(carrierCompanyId, currentUserCompanyId, currentUserProfileId);
            return Created("", appointment);
        }

        [HttpPut("{carrierCompanyId}/appointment")]
        public ActionResult<CarrierAppointment> UpdateAppointment([FromRoute] Guid carrierCompanyId, [FromForm(Name ="notes")] string brokerNotes)
        {
            Guid currentUserCompanyId = User.CurrentUserCompanyId();

            carrierAppointmentService.UpdateCarrierAppointment(carrierCompanyId, currentUserCompanyId, brokerNotes );
            return NoContent();
        }

        [HttpDelete("{carrierCompanyId}/appointment")]
        public ActionResult<CarrierAppointment> RemoveAppointment([FromRoute] Guid carrierCompanyId )
        {
            Guid currentUserCompanyId = User.CurrentUserCompanyId();

            carrierAppointmentService.RemoveCarrierAppointment(carrierCompanyId, currentUserCompanyId);
            return NoContent();
        }


    }
}
