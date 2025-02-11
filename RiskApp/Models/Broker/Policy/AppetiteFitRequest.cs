using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Models.Broker.Policy
{
    public class AppetiteFitRequest
    {
        public string Policy { get; set; }

        public string[] InsuranceTypes { get; set; }
    }
}
