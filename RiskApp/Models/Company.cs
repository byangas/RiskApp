using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Models
{
    public class Company
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DomainName { get; set; }
        // broker, carrier, etc.
        public CompanyType CompanyType { get; set; }

        public string Logo { get; set; }

    }
}
