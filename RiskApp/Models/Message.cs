using RiskApp.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Models
{

    public class Message
    {
        public Guid Id { get; set; }
        public Guid SubjectId { get; set; }
        public MessageType MessageType { get; set; }
   

        public string MessageContent { get; set; }

        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public Guid SentToProfileId { get; set; }
        public Guid SenderProfileId { get; set; }
        public DateTime? ReadDate { get;  set; }
    }
}
