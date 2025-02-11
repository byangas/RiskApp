using System;

namespace RiskApp.Services
{
    public class ProfileContact
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid ProfileId { get; set; }

        public DateTime? InvalidDate { get; set; }
    }
}