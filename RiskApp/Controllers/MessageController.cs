using Microsoft.AspNetCore.Mvc;
using RiskApp.Extensions;
using RiskApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Controllers
{
    [Route("[controller]")]
    public class MessageController : Controller
    {
        private readonly MessageService messageService;

        public MessageController(MessageService messageService)
        {
            this.messageService = messageService;
        }

        [HttpGet("{messageId}")]
        public IActionResult Index( [FromRoute] Guid messageId)
        {

            Models.MessageView message = messageService.GetMessageView(messageId);
            if(message == null || message.Message.ReadDate != null)
            {
                return View("InvalidMessage");
            }

            return View(message);
        }

        [HttpPost("{messageId}")] 
        public IActionResult Reply([FromRoute] Guid messageId, [FromForm] string reply)
        {
            Models.Message message = messageService.GetMessage(messageId);
           if(message == null || message.ReadDate != null)
            {
                return View("InvalidMessage");
            }

            messageService.ReplyToMessage(message, reply, Request.BaseUrl());


            return View();
        }
    }
}
