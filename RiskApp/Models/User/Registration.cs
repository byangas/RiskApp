using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Models.User
{
    public class Registration
    {
        public Guid Id { get; set; }
        public string RegistrationCode { get; set; }
        public Guid[] Roles { get; set; }

        public Guid CompanyId { get; set; }
        public string Email { get; set; }
    }
}
