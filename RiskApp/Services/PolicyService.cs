using RiskApp.Data;
using RiskApp.Models.Broker;
using RiskApp.Models.Broker.Policy;
using RiskApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Services
{

    public class PolicyService
    {
        private readonly PolicyRepository policyRepository;
        private readonly ConnectionManager connectionManager;

        public PolicyService(Repositories.PolicyRepository policyRepository, ConnectionManager connectionManager)
        {
            this.policyRepository = policyRepository ?? throw new ArgumentNullException(nameof(policyRepository));
            this.connectionManager = connectionManager;
        }

        public Guid CreatePolicy(Guid customerId, PolicyEdit policy, Guid currentUserId)
        {
            return policyRepository.CreatePolicy(customerId, policy, currentUserId);
        }

        public IEnumerable<dynamic> GetCustomerPolicies(Guid brokerAccountId)
        {
            return policyRepository.GetPolicies(brokerAccountId);
        }

        public dynamic GetPolicy(Guid policyId)
        {
            return policyRepository.GetPolicy(policyId);
        }




        public Guid? CreateMarketingSheetContact(Guid policyId, Guid contactProfileId, Guid currentUserId)
        {
            return policyRepository.CreateMarketingSheetContact(policyId, contactProfileId, currentUserId);
        }

        public IEnumerable<PolicySummary> GetPoliciesForCompany(Guid companyId, string orderBy)
        {
            return policyRepository.GetCompanyPolicies(companyId, orderBy);
        }

        public void UpdateAppetite(Guid policyId, AppetiteFitRequest appetiteFitRequest)
        {
            policyRepository.UpdateAppetite(policyId, appetiteFitRequest.Policy, appetiteFitRequest.InsuranceTypes);
        }

        public void CreatePolicyNote(Guid policyId, string note, Guid createdBy)
        {
            policyRepository.CreatePolicyNote(policyId, note, createdBy);
        }

        public IEnumerable<PolicyNote> GetPolicyNotes(Guid policyId)
        {
            return policyRepository.GetPolicyNotes(policyId);
        }

        public MarketingSheetContact GetMarketingSheetContact(Guid marketingSheetContactId)
        {
            return policyRepository.GetMarketingSheetContact(marketingSheetContactId);
        }

        public void UpdatePolicyDetails(Guid policyId, string description, DateTime? renewalDate)
        {
            policyRepository.UpdatePolicyDetails(policyId, description, renewalDate);
        }

        public IEnumerable<MarketingSheetContact> GetMarketingSheet(Guid policyId)
        {
            return policyRepository.GetMarketingSheet(policyId);
        }

        public void DeleteMarketingSheetContact(Guid marketingSheetContactId)
        {
            policyRepository.DeleteMarketingSheetContact(marketingSheetContactId);
        }

        public void DeletePolicy(Guid policyId)
        {
            connectionManager.BeginTransaction();

            try
            {
                policyRepository.DeleteMarketingSheetNotesForPolicy(policyId);
                policyRepository.DeleteMarketingSheetContactsForPolicy(policyId);
                policyRepository.DeletePolicy(policyId);
                connectionManager.CommitTransaction();
            }
            catch (Exception)
            {
                connectionManager.Rollback();
                throw;
            }

        }

        public void UpdateMarketingSheetContactStatus(Guid marketingSheetContactId, MarketingSheetContactStatus status)
        {
            policyRepository.UpdateMarketingSheetField(marketingSheetContactId, "status", status.ToString());
        }

        public void UpdateMarketingSheetContactCommission(Guid marketingSheetContactId, double commision)
        {
            policyRepository.UpdateMarketingSheetField(marketingSheetContactId, "commission", commision);
        }

        public void AddMarketingSheetContactNote(Guid marketingSheetContactId, string note)
        {
            policyRepository.AddMarketingSheetContactNote(marketingSheetContactId, note);
        }

        public IEnumerable<MarketingSheetContactNote> MarketingSheetContactNotes(Guid marketingSheetContactId)
        {
            return policyRepository.MarketingSheetContactNotes(marketingSheetContactId);
        }

        public void UpdateMarketingSheetPremium(Guid marketingSheetContactId, int premium)
        {
            policyRepository.UpdateMarketingSheetField(marketingSheetContactId, "premium", premium);
        }

        public void UpdateMarketingSheetCommission(Guid marketingSheetContactId, int commission)
        {
            policyRepository.UpdateMarketingSheetField(marketingSheetContactId, "commission", commission);
        }

    }
}
