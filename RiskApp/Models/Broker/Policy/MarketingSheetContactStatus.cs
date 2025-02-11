using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Models.Broker.Policy
{
    public enum MarketingSheetContactStatus
    {
        @new,
        pendingMfr,
        noFit,
        quote,
        pendingQuote,
        received,
        blocked,
        invalid
    }
}
