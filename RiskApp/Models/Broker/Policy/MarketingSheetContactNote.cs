using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Models.Broker.Policy
{
    public class MarketingSheetContactNote
    {
        public string Note { get; set; }
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
    }
}
