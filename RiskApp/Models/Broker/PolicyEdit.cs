using System;
using System.Text.Json.Serialization;
namespace RiskApp.Models.Broker
{


    public class PolicyEdit
    {
        public PolicyEdit()
        {
            InsuranceTypes = new string[0];
        }
        public string Detail { get; set; }
        
        public string[] InsuranceTypes { get; set; }

        public string Status { get; set; }

        public DateTime? RenewalDate { get; set; }

        public string Description { get; set; }
    }
}