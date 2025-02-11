using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RiskApp.Models.Broker;
using RiskApp.Services;
using RiskApp.Extensions;

namespace RiskApp.Controllers.API
{
    [Authorize]
    [Route("api/broker/customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService customerService;
        private readonly PolicyService policyService;

        public CustomerController(CustomerService customerService, PolicyService policyService)
        {
            this.customerService = customerService;
            this.policyService = policyService;
        }

        [HttpPost]
        public ActionResult Create([FromBody] Customer customer)
        {
            Guid currentUserId = User.CurrentUserId();
            Guid companyId = User.CurrentUserCompanyId();
            Customer created = customerService.Create(customer, companyId, currentUserId);
            return Created("", created);
        }


        [HttpPut("{customerId}")]
        public ActionResult Update([FromRoute] Guid customerId, [FromBody] Customer customer)
        {
            Guid currentUserId = User.CurrentUserId();

            customerService.Update(customer, customerId, currentUserId);
            return NoContent();
        }

        [HttpGet]
        public List<Customer> AllCustomers()
        {
            Guid currentCompanyId = User.CurrentUserCompanyId();
            return customerService.CompanyCustomers(currentCompanyId);
        }

        [HttpGet("{id}")]
        public Customer GetCustomer([FromRoute] Guid id)
        {
            return customerService.GetCustomerById(id);
        }

        [HttpDelete("{id}")]
        public void Delete([FromRoute] Guid id)
        {
            customerService.Delete(id);
        }


        [HttpGet("{id}/policy")]
        public IEnumerable<dynamic> GetPolicies([FromRoute] Guid id)
        {
            return policyService.GetCustomerPolicies(id);
        }


    }
}
