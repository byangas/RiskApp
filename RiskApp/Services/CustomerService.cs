using RiskApp.Data;
using RiskApp.Models.Broker;
using RiskApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Services
{
    public class CustomerService
    {
        private readonly ConnectionManager connectionManager;
        private readonly CustomerRepository customerRepository;

        public CustomerService(ConnectionManager connectionManager, CustomerRepository brokerAccountRepository)
        {
            this.connectionManager = connectionManager;
            this.customerRepository = brokerAccountRepository;
        } 
        public Customer Create(Customer customer, Guid companyId, Guid currentUserId)
        {
            return customerRepository.Create(customer, companyId, currentUserId);
        }

        public List<Customer> CompanyCustomers(Guid companyId)
        {
            return customerRepository.CompanyCustomers(companyId);
        }

        public Customer GetCustomerById(Guid customerId)
        {
            return customerRepository.CustomerById(customerId);
        }

        public void Delete(Guid id)
        {
            try
            {
                connectionManager.BeginTransaction();
                // wrapped in transaction because this is a cascading delete and will delete
                // all policies for this account as well.
                customerRepository.Delete(id);
                connectionManager.CommitTransaction();
            }
            catch (Exception)
            {
                connectionManager.Rollback();
                throw;
            }
        }

        public void Update(Customer customer, Guid customerId, Guid currentUserId)
        {
            customerRepository.Update(customer, customerId, customerId);
        }
    }
}
