using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiskApp.Models;
using RiskApp.Services;
using RiskApp.Extensions;
using System;
using System.Collections.Generic;

namespace RiskApp.Controllers.API
{
    [Authorize]
    [Route("api/message")]
    public class MessageController : ControllerBase
    {
        private readonly MessageService messageService;
        public MessageController(MessageService messageService)
        {
            this.messageService = messageService;
        }

        [HttpGet("subject/{subjectId}")]
        public IEnumerable<Message> GetMessageThread([FromRoute] Guid subjectId)
        {
            return messageService.GetMessageThread(subjectId);
        }

        [HttpPost]
        public Guid SendMessage([FromForm] Guid recipientId, [FromForm] Guid subjectId, [FromForm] string message)
        {
            return messageService.SendMessage(User.CurrentUserId(), recipientId, subjectId, message, Request.BaseUrl());
        }

        [HttpPut("read/{messageId}")]
        public IActionResult MarkAsRead([FromRoute] Guid messageId)
        {
            Message message = messageService.GetMessage(messageId);
            if (message != null)
            {
                messageService.MarkAsRead(messageId);
            }
            return NoContent();
        }

        [HttpPost("reply/{messageId}")]
        public IActionResult ReplyToMessage([FromRoute] Guid messageId, [FromForm] string content)
        {
            Message message = messageService.GetMessage(messageId);
            if (message == null)
            {
                return NotFound();
            }

            messageService.ReplyToMessage(message, content, Request.BaseUrl());
            return NoContent();
        }

        [HttpGet("unread")]
        public IEnumerable<MessageView> CarrierMessages()
        {
            return messageService.CarrierMessages(User.CurrentUserId());
        }

    }

}
