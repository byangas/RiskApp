using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RiskApp.Extensions;
using RiskApp.Models.Broker;
using RiskApp.Models.Broker.Policy;
using RiskApp.Services;

namespace RiskApp.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PolicyController : ControllerBase
    {
        private readonly PolicyService policyService;
        private readonly MessageService messageService;

        public PolicyController(PolicyService policyService, MessageService messageService)
        {
            this.policyService = policyService;
            this.messageService = messageService;
        }

        /// <summary>
        /// Gets the list of all the policies for the current users company
        /// </summary>
        /// <returns></returns>
        [HttpGet()] 
        public IEnumerable<PolicySummary> GetPoliciesForCompany([FromQuery] string orderBy)
        {
            Guid companyId = User.CurrentUserCompanyId();
           return policyService.GetPoliciesForCompany(companyId, orderBy);
        }

        [HttpPost("customer/{id}")]
        public Guid CreatePolicy([FromRoute] Guid id, PolicyEdit policy)
        {

            Guid currentUserId = User.CurrentUserId();

            return policyService.CreatePolicy(id, policy, currentUserId);
        }

        [HttpPost("{policyId}/note")]
        public ActionResult CreatePolicyNote([FromRoute] Guid policyId, [FromForm] string note)
        {
            if (string.IsNullOrEmpty(note))
                return NoContent();

            Guid createdBy = User.CurrentUserId();
            policyService.CreatePolicyNote(policyId, note, createdBy);

            return NoContent();
        }

        [HttpGet("{policyId}/note")]
        public IEnumerable<PolicyNote> GetPolicyNotes([FromRoute] Guid policyId)
        {
            return policyService.GetPolicyNotes(policyId);
        }

        [HttpDelete("{policyId}/appetite")]
        public ActionResult DeleteAppetite([FromRoute] Guid policyId)
        {

            policyService.UpdateAppetite(policyId, new AppetiteFitRequest());
            return NoContent();
        }
        [HttpPut("{policyId}/details")] 
        public ActionResult UpdatePolicyDetails([FromRoute] Guid policyId, [FromForm] string description, [FromForm] DateTime? renewalDate )
        {
            policyService.UpdatePolicyDetails(policyId, description, renewalDate);
            return NoContent();
        }
           
        [HttpPut("{policyId}/appetite")]
        public ActionResult UpdateAppetite([FromRoute] Guid policyId, [FromBody] AppetiteFitRequest appetiteFitRequest)
        {
            policyService.UpdateAppetite(policyId, appetiteFitRequest);
            return NoContent();
        }

        [HttpDelete("{policyId}")]
        public ActionResult DeletePolicy([FromRoute] Guid policyId)
        {
            policyService.DeletePolicy(policyId);
            return NoContent();
        }

        [HttpGet("{policyId}")]
        public ActionResult GetPolicy([FromRoute] Guid policyId)
        {
            dynamic policy = policyService.GetPolicy(policyId);

            if (policy == null)
            {
                return NoContent();
            }

            return Ok(policy);
        }


        /// <summary>
        /// creates a record for the marketing sheet
        /// </summary>
        /// <param name="brokerAccountId"></param>
        /// <param name="policyId"></param>
        /// <param name="contactId"></param>
        /// <returns>MarketSheetContact ID</returns>
        [HttpPut("{policyId}/contact/create/{profileId}")]
        public ActionResult CreateMarketContact([FromRoute] Guid policyId, [FromRoute] Guid profileId)
        {
            Guid currentUserId = User.CurrentUserId();
            //if a duplicate contact is added, we just ignore. Dups will result in Null return value
            Guid? marketingContactId = policyService.CreateMarketingSheetContact(policyId, profileId, currentUserId);
            if (marketingContactId.HasValue)
            {
                return Created("", marketingContactId);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpGet("{policyID}/marketingsheet/")]
        public IEnumerable<MarketingSheetContact> GetMarketingSheet([FromRoute] Guid policyId)
        {
            return policyService.GetMarketingSheet(policyId);
        }

        [HttpDelete("marketingsheet/{marketingSheetContactId}")]
        public ActionResult Delete([FromRoute] Guid marketingSheetContactId)
        {
            policyService.DeleteMarketingSheetContact(marketingSheetContactId);
            return NoContent();
        }

        [HttpPut("marketingsheet/{marketingSheetContactId}/status")]
        public ActionResult Delete([FromRoute] Guid marketingSheetContactId, [FromForm] MarketingSheetContactStatus status)
        {
            policyService.UpdateMarketingSheetContactStatus(marketingSheetContactId, status);
            return NoContent();
        }


        [HttpPut("marketingsheet/{marketingSheetContactId}/note")]
        public ActionResult AddNote([FromRoute] Guid marketingSheetContactId, [FromForm] string note)
        {
            policyService.AddMarketingSheetContactNote(marketingSheetContactId, note);
            return NoContent();
        }

        [HttpPut("marketingsheet/{marketingSheetContactId}/premium")]
        public ActionResult UpdateMarketingSheetPremium([FromRoute] Guid marketingSheetContactId, [FromForm] int premium)
        {
            policyService.UpdateMarketingSheetPremium(marketingSheetContactId, premium);
            return NoContent();
        }


        [HttpPut("marketingsheet/{marketingSheetContactId}/commission")]
        public ActionResult UpdateMarketingSheetCommission([FromRoute] Guid marketingSheetContactId, [FromForm] int commission)
        {
                policyService.UpdateMarketingSheetCommission(marketingSheetContactId, commission);
            return NoContent();
        }

        [HttpPost("marketingsheet/{marketingSheetContactId}/afr")]
        public async Task<ActionResult> SendAppetiteFitRequestAsync([FromRoute] Guid marketingSheetContactId, [FromForm] string message)
        {
            string baseUrl = Request.BaseUrl();
            Guid currentUser = User.CurrentUserId();
            await messageService.SendAppetiteFitRequestAsync(marketingSheetContactId, baseUrl, message, currentUser);

            return NoContent();
        }

        [HttpGet("marketingsheet/{marketingSheetContactId}/note")]
        public IEnumerable<MarketingSheetContactNote> MarketingSheetContactNotes([FromRoute] Guid marketingSheetContactId)
        {
            return policyService.MarketingSheetContactNotes(marketingSheetContactId);
        }

    }
}
