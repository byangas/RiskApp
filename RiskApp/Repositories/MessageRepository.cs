using RiskApp.Data;
using RiskApp.Models;
using RiskApp.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace RiskApp.Repositories
{
    public class MessageRepository : RepositoryBase
    {
        public MessageRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public Guid CreateMessage(Guid sendToProfileId, Guid senderProfileId, string message, MessageType messageType, Guid subjectId)
        {
            switch (messageType)
            {
                case MessageType.Default:
                    break;
                case MessageType.AppetiteFitRequest:
                    break;
                case MessageType.AppetiteFitResponse:
                    break;
                default:
                    break;
            }
            var SQL = @" INSERT INTO public.message( sender_profile_id, send_to_profile_id, subject_id, message_type, message)
    VALUES(@SenderProfileId, @SendToProfileId, @SubjectId, @MessageType, @MessageContent) returning message_id;";
            var queryParams = new Dictionary<string, object>() {
                { "SenderProfileId", senderProfileId },
                { "SendToProfileId", sendToProfileId },
                { "SubjectId", subjectId },
                { "MessageContent", DBSafeNull(message)},
                { "MessageType", messageType.ToString() }
            };

            return ExecuteScalar<Guid>(SQL, queryParams);
        }

        public MessageView GetMessageView(Guid messageId)
        {
            List<MessageView> messages = new List<MessageView>();

            string SQL = @"Select message.*, first_name,last_name, company_name FROM message
JOIN profile on  profile.profile_id = message.sender_profile_id 
JOIN company on profile.company_id = company.company_id
WHERE message_id = @MessageId ;";
            using NpgsqlDataReader reader = ExecuteReader(SQL, "MessageId", messageId);
            if (reader.Read())
            {
                var messageView = new MessageView();
                messageView.Message = MessageFromReader(reader);
                messageView.SentBy = $"{reader.GetValue<string>("first_name")} {reader.GetValue<string>("last_name")} at {reader.GetValue<string>("company_name")}";
                messages.Add(messageView);
                return messageView;
            }
            // not found
            return null;
        }
        public Message GetMessage(Guid messageId)
        {
            string SQL = @"SELECT message_id, sender_profile_id, send_to_profile_id, subject_id, message_type, message, created_date, status, read_date from message WHERE message_id = @MessageId";

            using Npgsql.NpgsqlDataReader reader = ExecuteReader(SQL, "MessageId", messageId);
            if (reader.Read())
            {
                return MessageFromReader(reader);
            }

            //not found
            return null;
        }

        public IEnumerable<Message> GetMessageThreadOnSubject(Guid subjectId)
        {
            var messages = new List<Message>();
            string SQL = @"SELECT message_id, sender_profile_id, send_to_profile_id, subject_id, message_type, message, created_date, status, read_date from message WHERE subject_id = @SubjectId order by created_date";

            using Npgsql.NpgsqlDataReader reader = ExecuteReader(SQL, "SubjectId", subjectId);
            while (reader.Read())
            {
                messages.Add(MessageFromReader(reader));
            }

            return messages;
        }

        public IEnumerable<MessageView> CarrierMessages(Guid currentUserProfileId)
        {
            List<MessageView> messages = new List<MessageView>();

            string SQL = @"Select customer.firm_name as subject, message.*, first_name,last_name, company_name FROM message
JOIN profile on  profile.profile_id = message.sender_profile_id 
JOIN company on profile.company_id = company.company_id
LEFT JOIN public.marketing_sheet_contact MCT on MCT.marketing_sheet_contact_id = message.subject_id
LEFT JOIN policy on MCT.policy_id = policy.policy_id
LEFT JOIN customer on policy.customer_id = customer.customer_id
WHERE send_to_profile_id = @CurrentUserId and message.read_date is null ORDER BY message.created_date;";
            using NpgsqlDataReader reader = ExecuteReader(SQL, "CurrentUserId", currentUserProfileId);
            while (reader.Read())
            {
                var messageView = new MessageView();
                messageView.Subject = reader.GetValue<string>("subject");
                messageView.Message = MessageFromReader(reader);
                messageView.SentBy = $"{reader.GetValue<string>("first_name")} {reader.GetValue<string>("last_name")} at {reader.GetValue<string>("company_name")}";
                messages.Add(messageView);
            }
            return messages;
        }

        private static Message MessageFromReader(NpgsqlDataReader reader)
        {
            return new Message()
            {
                Id = reader.GetValue<Guid>("message_id"),
                CreatedDate = reader.GetValue<DateTime>("created_date").ToUniversalTime(),
                Status = reader.GetValue<string>("status"),
                MessageType = reader.GetValue<MessageType>("message_type"),
                MessageContent = reader.GetValue<string>("message"),
                SubjectId = reader.GetValue<Guid>("subject_id"),
                SentToProfileId = reader.GetValue<Guid>("send_to_profile_id"),
                SenderProfileId = reader.GetValue<Guid>("sender_profile_id"),
                ReadDate = reader.GetValue<DateTime?>("read_date")
            };
        }

        public void MarkAsRead(Guid messageId)
        {
            var SQL = @"Update message set read_date = CURRENT_TIMESTAMP WHERE message_id = @MessageID";
            ExecuteNonQuery(SQL, "MessageId", messageId);
        }
    }
}
