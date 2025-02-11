using System;

namespace RiskApp.Models.User
{
    // used to get basic information about a user that
    // can be used for API calls
    public class UserInfo : ProfileWithCompany
    {
        public string[] Roles { get; set; }
    }
}

