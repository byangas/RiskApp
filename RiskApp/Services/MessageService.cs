using RiskApp.Data;
using RiskApp.Models;
using RiskApp.Models.Broker.Policy;
using RiskApp.Models.User;
using RiskApp.Repositories;
using RiskApp.Utility;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Mail;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace RiskApp.Services
{
    public enum AppetiteFitResponse
    {
        Accept, Deny
    }
    public class MessageService
    {
        private readonly PolicyService policyService;
        private readonly ProfileService profileService;
        private readonly CustomerService customerService;

        internal MessageView GetMessageView(Guid messageId)
        {
            return messageRepository.GetMessageView(messageId);
        }

        private readonly ConnectionManager connection;
        private readonly MessageRepository messageRepository;

        public MessageService(
            PolicyService policyService,
            ProfileService profileService,
            CustomerService brokerAccountService,
            ConnectionManager connection,
            MessageRepository messageRepository,
            EmailRenderEngine emailRenderEngine, IHostEnvironment environment)
        {
            this.policyService = policyService;
            this.profileService = profileService;
            this.customerService = brokerAccountService;
            this.connection = connection;
            this.messageRepository = messageRepository;
            this.emailRenderEngine = emailRenderEngine;
            this.environment = environment;
        }

        public void ReplyToMessage(Message message, string reply, string baseUrl)
        {

                //reply to the original sender, with the recipient now becoming the "sender"
                SendMessage(message.SentToProfileId, message.SenderProfileId, message.SubjectId, reply, baseUrl);
                
                messageRepository.MarkAsRead(message.Id);
        }

        public Guid SendMessage(Guid fromUserId, Guid recipientId, Guid subjectId, string message, string baseUrl)
        {
            try
            {
                connection.BeginTransaction();
                Guid messageId = messageRepository.CreateMessage(recipientId, fromUserId, message, MessageType.Default, subjectId);

                UserInfo fromUserInfo = profileService.GetUserInfo(fromUserId);
                UserInfo recipient = profileService.GetUserInfo(recipientId);
                string emailContent = GenerateEmailContent(messageId, message, fromUserInfo, baseUrl);
                SendEmail(recipient, fromUserInfo, $"New Message from {fromUserInfo.Name} using Riskminute.com.", emailContent);

                connection.CommitTransaction();
                return messageId;
            }
            catch (Exception)
            {
                connection.Rollback();
                throw;
            }
        }

        public IEnumerable<MessageView> CarrierMessages(Guid currentUserProfileId)
        {
            return messageRepository.CarrierMessages(currentUserProfileId);
        }

        private readonly EmailRenderEngine emailRenderEngine;
        private readonly IHostEnvironment environment;

        public bool ProcessAppetiteFitResponse(Guid messageId, AppetiteFitResponse response, string baseUrl)
        {
            Message message = GetMessage(messageId);
            if (message == null || message.ReadDate != null)
            {
                return false;
            }
            Guid senderProfileId = message.SenderProfileId;

            MarketingSheetContact marketingSheetContact = policyService.GetMarketingSheetContact(message.SubjectId);

            // if the status of the line item (contact) is not waiting for the response, because the response has already been
            // recieved, etc., them mark the message as "read" and move on.
            if (marketingSheetContact.Status != MarketingSheetContactStatus.pendingMfr)
            {
                // mark message as 'read'
                MarkAsRead(message.Id);
                return false;
            }

            MarketingSheetContactStatus newStatus = response == AppetiteFitResponse.Accept ? MarketingSheetContactStatus.quote : MarketingSheetContactStatus.noFit;

            try
            {
                connection.BeginTransaction();
                string messageContent = "";

                if (response == AppetiteFitResponse.Accept)
                {
                    messageContent = "Yes, this fits our appetite. Please apply.";
                }
                else if (response == AppetiteFitResponse.Deny)
                {
                    messageContent = "Does not fit our current appetite.";
                }

                // mark message as 'read'
                MarkAsRead(message.Id);

                policyService.UpdateMarketingSheetContactStatus(marketingSheetContact.Id, newStatus);

                Guid newMessageId = messageRepository.CreateMessage(senderProfileId, marketingSheetContact.ProfileId, messageContent, MessageType.AppetiteFitResponse, marketingSheetContact.Id);


                // userInfo for the recipient, which in this case is the Broker marketing the Policy
                UserInfo recipientUserInfo = profileService.GetUserInfo(senderProfileId);
                // get UserInfo on "sender"
                UserInfo fromUserInfo = profileService.GetUserInfo(marketingSheetContact.ProfileId);
                string emailContent = GenerateEmailContent(newMessageId, messageContent, fromUserInfo, baseUrl);

                SendEmail(recipientUserInfo, fromUserInfo, $"{fromUserInfo.Name} has responded to your Appetite Fit Request", emailContent);

                connection.CommitTransaction();
            }
            catch (Exception)
            {
                connection.Rollback();
                throw;
            }
            return true;
        }

        private string GenerateEmailContent(Guid messageId, string messageContent, UserInfo fromUserInfo, string baseUrl)
        {
            dynamic model = new ExpandoObject();
            model.message = messageContent;
            model.user = fromUserInfo;
            model.baseUrl = baseUrl;
            model.messageId = messageId;


            string emailContent = emailRenderEngine.RenderViewToStringAsync("Views\\EmailTemplates\\Message.cshtml", model).Result;
            return emailContent;
        }

        public IEnumerable<Message> GetMessageThread(Guid subjectId)
        {
            return messageRepository.GetMessageThreadOnSubject(subjectId);
        }

        public Message GetMessage(Guid messageId)
        {
            return messageRepository.GetMessage(messageId);
        }

        public async Task SendAppetiteFitRequestAsync(Guid marketingSheetContactId, string baseUrl, string message, Guid currentUserProfileId)
        {
            try
            {
                connection.BeginTransaction();

                MarketingSheetContact marketingSheetContact = policyService.GetMarketingSheetContact(marketingSheetContactId);

                Guid messageId = messageRepository.CreateMessage(marketingSheetContact.ProfileId, currentUserProfileId, message, MessageType.AppetiteFitRequest, marketingSheetContactId);

                policyService.UpdateMarketingSheetContactStatus(marketingSheetContactId, MarketingSheetContactStatus.pendingMfr);

                dynamic model = CreateMarketFitRequestModel(marketingSheetContact.PolicyId, baseUrl, message, messageId);

                string emailContent = await emailRenderEngine.RenderViewToStringAsync("Views\\EmailTemplates\\MarketFitRequest.cshtml", model);
                UserInfo to = profileService.GetUserInfo(marketingSheetContact.ProfileId);

                SendEmail(to, (UserInfo)model.user, $"Appetite Fit Request from {model.user.Name}", emailContent);

                connection.CommitTransaction();
            }
            catch (Exception ex)
            {
                connection.Rollback();
                throw;
            }

        }

        public void MarkAsRead(Guid messageId)
        {
            messageRepository.MarkAsRead(messageId);
        }


        private void SendEmail(UserInfo to, UserInfo from, string subject, string content)
        {
   
            if(environment.IsDevelopment())         {
                SendEmailLocalHost(to, from, subject, content);
                return;
            }
            // only send emails out for local development or in production (for the time being)
            if(!environment.IsProduction())
            {
                return;
            }
            // this is not a real key, since we don't check in "secrets" ;-)
            SendGrid.SendGridClient client = new SendGrid.SendGridClient(String.Empty);
      
            var msg = new SendGrid.Helpers.Mail.SendGridMessage()
            {
                From = new SendGrid.Helpers.Mail.EmailAddress("noreply@riskminute.com", "RiskMinute.com Message"),
                Subject = subject,
                HtmlContent = content,
                ReplyTo = new SendGrid.Helpers.Mail.EmailAddress(from.Email, from.Name),
                
            };
            msg.AddTo(to.Email, to.Name);
            var response = client.SendEmailAsync(msg).Result;
            if(!response.IsSuccessStatusCode)
            {
                string test = response.Body.ReadAsStringAsync().Result;
                throw new Exception(test);
            }
        }

        private static void SendEmailLocalHost(UserInfo to, UserInfo from, string subject, string content)
        {
            /*************************** 
             * if you want to test by sending through Gmail account (instead of SendGrid or use PaperCut SMTP) 
             * requires updating security on your Gmail account to allow for this
             * 
            SmtpClient smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential("<youremail>@gmail.com", "<yourpassword>")
            };
            ********/

            SmtpClient smtpClient = new SmtpClient() { EnableSsl = false, Host = "localhost", Port = 25 };
            MailMessage message = new MailMessage(from: "noreply@riskminute.com", to: to.Email, subject: subject, body: content)
            {
                IsBodyHtml = true
            };

            message.ReplyToList.Add(new MailAddress(from.Email, from.Name));
            smtpClient.Send(message);
        }

        public void SendRegistrationCode(string registrationEmail, string registrationCode, DateTime expiresAt, string baseUrl)
        {
            // create parameters to pass into the razor rendering for the email template
            dynamic model = new System.Dynamic.ExpandoObject();
            model.registrationCode = registrationCode;
            // todo: add message to Email that tells the user that the registration code expires
            model.expiresAt = expiresAt;
            model.email = registrationEmail;
            model.baseUrl = baseUrl;

            //have to create some "fake" Users to send email to/from
            UserInfo newUser = new UserInfo() { Email = registrationEmail, Name = "" };
            UserInfo from = new UserInfo() { Email = "noreply@riskminute.com", Name = "RiskMinute.com" };
            string emailContent = emailRenderEngine.RenderViewToStringAsync(@"Views\EmailTemplates\Registration.cshtml", model).Result;

            SendEmail(newUser, from, "RiskMinute.com Registration Code", emailContent);

        }


        public dynamic CreateMarketFitRequestModel(Guid policyId, string baseUrl, string message, Guid messageId)
        {
            dynamic policy = policyService.GetPolicy(policyId);
            var user = profileService.GetUserInfo(policy.createByProfileId);
            var account = customerService.GetCustomerById(policy.customerId);

            dynamic policyDetail = policy.detail;
            string policyNotes = null;
            if(policyDetail.ContainsKey("additionalInformation"))
            {
                JsonElement notes = (JsonElement)policyDetail["additionalInformation"];
                policyNotes = notes.ToString();  
            }

            Dictionary<string, object> lines = MapFromJsonElementProperties((JsonElement)policyDetail["insurance"]);

            dynamic model = new ExpandoObject();
            model.user = user;
            model.account = account;
            model.insurance = lines;
            model.baseUrl = baseUrl;
            model.message = message;
            model.messageId = messageId;
            model.policyNotes = policyNotes;
            return model;
        }


        /// <summary>
        /// Takes a JSONElement as root and creates a dictionary of the properties of the element. Only handles scenario where
        /// The root has properties, and those properties are complex objects with either numbers or strings. The main reason
        /// for this function is because there is no easy way to turn JSON into a key/value pair structure, as is possible in Java
        /// and other platforms.
        /// </summary>
        /// <param name="root"></param>
        /// <returns>Dictionary from the JsonElement</returns>
        private static Dictionary<string, object> MapFromJsonElementProperties(JsonElement root)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            try
            {

                foreach (var node in root.EnumerateObject())
                {
                    Dictionary<string, object> values = new Dictionary<string, object>();
                    foreach (var element in node.Value.EnumerateObject())
                    {
                        if (element.Value.ValueKind == JsonValueKind.Number)
                        {
                            values.Add(element.Name, element.Value.GetInt32());
                        }
                        else
                        {
                            values.Add(element.Name, element.Value.GetString());
                        }
                    }
                    properties.Add(node.Name, values);
                }
            }
            catch
            {
                //do nothing intentionally. 
            }

            return properties;
        }
    }
}
