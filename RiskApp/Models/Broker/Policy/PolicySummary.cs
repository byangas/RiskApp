using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Models.Broker.Policy
{
    public class PolicySummary
    {
        public Guid PolicyId { get; set; }
        public string Status { get; set; }

        public string Description { get; set; }
        public string BrokerName { get; set; }
        public string Customer { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? RenewalDate { get; set; }

        public Int64 MarketingSheetCount { get; set; }
        public Guid CustomerId { get; set; }
    }
}
