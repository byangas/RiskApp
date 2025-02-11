using RiskApp.Data;
using RiskApp.Models;
using RiskApp.Models.System;
using RiskApp.Models.User;
using RiskApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiskApp.Services
{
    public class RegistrationService
    {
        private readonly ConnectionManager connectionManager;
        private readonly MessageService messageService;
        private readonly RegistrationRepository registrationRepository;
        private readonly CompanyRepository companyRepository;
        private readonly ApplicationRoleRepository applicationRoleRepository;

        public RegistrationService(ConnectionManager connectionManager,
                                   MessageService messageService,
                                   RegistrationRepository registrationRepository,
                                   CompanyRepository companyRepository,
                                   ApplicationRoleRepository applicationRoleRepository)
        {
            this.connectionManager = connectionManager;
            this.messageService = messageService;
            this.registrationRepository = registrationRepository;
            this.companyRepository = companyRepository;
            this.applicationRoleRepository = applicationRoleRepository;
        }

        private static DateTime RegistrationExpiresAt()
        {
            // todo: make this configurable
            return DateTime.Now.AddDays(2);
        }

        private static string GenerateRegistrationCode()
        {
            Random random = new Random();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                sb.Append((char)random.Next(65, 90));
            }
            sb.Append(random.Next(1000, 99000));
            return sb.ToString();
        }

        public IEnumerable<ResponseMessage> SendRegistrationCode(string registrationEmail, string baseUrl)
        {

            List<ResponseMessage> messages = new List<ResponseMessage>();
            // validate email has domain.
            string[] split = registrationEmail.Split("@");
            if (split.Length != 2)
            {
                messages.Add(ResponseMessage.Create("Invalid email address"));
                return messages;
            }

            string emailDomain = split[1];
            Company company = companyRepository.GetCompanyByEmailDomain(emailDomain);
            if (company == null)
            {
                messages.Add(ResponseMessage.Create("Invalid email address. Only valid work emails are allowed. Gmail, Hotmail or other consumer email addresses are not supported at this time."));
                return messages;
            }

            List<Guid> roles = new List<Guid>();
            // get roles for user based on company type
            switch (company.CompanyType)
            {
                case CompanyType.broker:
                    // get broker roles
                    roles.Add(applicationRoleRepository.GetBrokerRoleId());
                    break;
                case CompanyType.carrier:
                    roles.Add(applicationRoleRepository.GetCarrierRoleId());
                    break;

            }

            string registrationCode = GenerateRegistrationCode();
            DateTime expiresAt = RegistrationExpiresAt();

            connectionManager.BeginTransaction();
            try
            {
                //delete all existing registration codes for user
                registrationRepository.DeleteAllByEmail(registrationEmail);

                //create registration in DB
                registrationRepository.CreateRegistration(registrationEmail, registrationCode, expiresAt, company.Id, roles.ToArray());

                // send the email now that all DB updates are done. If email fails, transaction will get rolled back.
                messageService.SendRegistrationCode(registrationEmail, registrationCode, expiresAt, baseUrl);

                connectionManager.CommitTransaction();
            }
            catch (Exception)
            {
                connectionManager.Rollback();
                throw;
            }
            messages.Add(new ResponseMessage() { Display = "Registration code sent to your email address" });
            return messages;

        }

        public Registration GetValidRegistration(string registrationEmail, string registrationCode)
        {
            return registrationRepository.GetValidRegistration(registrationEmail, registrationCode);
        }
    }
}
