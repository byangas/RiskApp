using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Models.User
{
    public class SignInInfo
    {
        public Guid AccountId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid ProfileId { get; set; }
        public string Password { get; set; }

    }
}
