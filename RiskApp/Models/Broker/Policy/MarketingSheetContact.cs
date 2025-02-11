using System;

namespace RiskApp.Models.Broker.Policy
{
    public class MarketingSheetContact
    {
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Logo { get; set; }
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public MarketingSheetContactStatus Status { get; set; }
        public int Commission { get; set; }

        public int Premium { get; set; }


        public Guid Id { get; set; }
        public Guid ProfileId { get; set; }
        public Guid PolicyId { get; set; }
        public Guid CreatedByProfileId { get; set; }
        public string MobilePhone { get;  set; }
    }
}