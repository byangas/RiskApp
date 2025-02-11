using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RiskApp.Extensions;
using RiskApp.Models;
using RiskApp.Services;

namespace RiskApp.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyService companyService;

        public CompanyController(CompanyService companyService)
        {
            this.companyService = companyService;
        }

        [HttpGet("{companyId}")]
        public ActionResult<Company> CompanyById([FromRoute] Guid companyId)
        {
            return Ok(companyService.GetCompanyById(companyId));
        }

        [HttpGet("")]
        public ActionResult<Company> CompanyByDomain([FromQuery] string emailDomain)
        {

            Company company = companyService.GetByEmailDomain(emailDomain.ToLowerInvariant());
            if (company == null)
            {
                return NoContent();
            }
            
            return Ok(company);
        }

        [HttpPost("")]
        public ActionResult<Company> CreateCarrierCompany(
            [FromForm(Name = "name")] string companyName, 
            [FromForm(Name = "domain")] string emailDomain)
        {

           Company company = companyService.CreateCarrierCompany(companyName, emailDomain.ToLowerInvariant());
       
            return Ok(company);
        }


        [HttpGet("carrier")]
        public ActionResult<IEnumerable<Company>> GetAllCarriers([FromQuery] string search, [FromQuery] bool appointed)
        {
            Guid currentUserCompanyId = User.CurrentUserCompanyId();

            return Ok(companyService.GetAllCarriers(search, appointed, currentUserCompanyId));
        }

    }
}
