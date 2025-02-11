using RiskApp.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Models
{
    public class Contact : ProfileWithCompany
    {
        /// <summary>
        /// Based on the current logged in user, whether the profile is a company contact.
        /// </summary>
        public bool CompanyContact { get; set; }
    }
}
