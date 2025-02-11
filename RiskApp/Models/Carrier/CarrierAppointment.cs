using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Models.Carrier
{
    public class CarrierAppointment
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreateByProfileId { get; set; }
        public string CreatedByName { get; set; }

        /// <summary>
        /// Notes that are specific (viewable) to the brokerage, but not anyone outside the brokerage
        /// </summary>
        public string BrokerNotes { get; set; }

    }
}
