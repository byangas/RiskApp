using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Models.Broker.Policy
{
    public class PolicyNote
    {
        public string Note { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public Guid  Id { get; set; }

    }
}
