using System;

namespace RiskApp.Models.User
{
    public class ProfileCreate
    {
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string MobilePhone { get; set; }
        public Guid CompanyId { get; set; }
        public string Title { get; set; }
        public string Specialty { get;  set; }
    }
}