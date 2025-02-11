using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Models.Broker
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string PrimaryContact { get; set; }

        public string Industry { get; set; }
        public string IndustryDetail  { get; set; }

        public string PrimaryContactPhone { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string Email { get; set; }

        public string AdditionalInformation { get; set; }
        public string CustomerName { get;  set; }
        public string FirmName { get;  set; }

        public Guid CustomerId { get; set; }
    }
}
