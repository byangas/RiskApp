using Microsoft.AspNetCore.Mvc;
using RiskApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Controllers
{
    public class TestController : Controller
    {
        private readonly CompanyService companyService;

        public TestController(CompanyService companyService)
        {
            this.companyService = companyService;
        }

        public IActionResult Company()
        {
            dynamic model = new System.Dynamic.ExpandoObject();
            IEnumerable<Models.Company> companies = new List<Models.Company>();// companyService.GetAllCarriers(null);
            model.companies = companies;
            return View(model);
        }
    }
}
