using Microsoft.AspNetCore.Mvc;
using RiskApp.Extensions;
using RiskApp.Models;
using RiskApp.Models.Broker.Policy;
using RiskApp.Models.User;
using RiskApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RiskApp.Controllers
{
    [Route("[controller]")]
    public class AppetiteFitRequestController : Controller
    {
        private readonly MessageService messageService;
        private readonly PolicyService policyService;

        public AppetiteFitRequestController(MessageService messageService, PolicyService policyService)
        {
            this.messageService = messageService;
            this.policyService = policyService;
        }



        [ResponseCache(NoStore = true)]
        [HttpGet("{messageId}/view")]
        public IActionResult Index([FromRoute] Guid messageId)
        {

            string baseUrl = Request.BaseUrl();
            Message message = messageService.GetMessage(messageId);
            // TODO: validate if the message is the wrong type, or has already been processed, etc. Show the user
            // a web page describing the issue
            MarketingSheetContact marketingSheetContact = policyService.GetMarketingSheetContact(message.SubjectId);

            dynamic model = messageService.CreateMarketFitRequestModel(marketingSheetContact.PolicyId, baseUrl, message.MessageContent, messageId);

            return View("Views\\EmailTemplates\\MarketFitRequest.cshtml", model);
        }



        [HttpGet("{messageId}/accept")]
        public IActionResult AcceptAFR([FromRoute] Guid messageId)
        {
            return UpdateMarketingSheet(messageId, AppetiteFitResponse.Accept);
        }

        [HttpGet("{messageId}/reject")]
        public IActionResult RejectAFR([FromRoute] Guid messageId)
        {
            return UpdateMarketingSheet(messageId, AppetiteFitResponse.Deny);
        }

        private IActionResult UpdateMarketingSheet(Guid messageId, AppetiteFitResponse response)
        {

            if (messageService.ProcessAppetiteFitResponse(messageId, response, Request.BaseUrl()))
            {
                return View("Thankyou");
            }
            return View("InvalidMessage");
        }
    }
}
