using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RiskApp.Extensions
{
    public static class ClaimsPrincipalExtention
    {
        public static Guid CurrentUserCompanyId(this ClaimsPrincipal principal)
        {
            string companyId = principal.Claims.Where(claim => claim.Type == "CompanyId")
                .Select(claim => claim.Value)
                .FirstOrDefault();

            if (companyId == null)
            {
                throw new Exception("Current user does not have a company Id set, so something is terribly wrong");
            }
            return new Guid(companyId);
        }

        public static Guid CurrentUserId(this ClaimsPrincipal principal)
        {
            return new Guid(principal.Identity.Name);
        }

    }
}
