using Microsoft.AspNetCore.Mvc;
using RiskApp.Models;
using RiskApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Services
{
    
    public class CompanyService
    {
        private readonly CompanyRepository companyRepository;

        public CompanyService(CompanyRepository companyRepository)
        {
            this.companyRepository = companyRepository;
        }

        public Company GetByEmailDomain(string emailDomain)
        {
            return companyRepository.GetCompanyByEmailDomain(emailDomain);
        }

        public IEnumerable<Company> GetAllCarriers(string search, bool appointedCarriers, Guid currentUserCompanyId)
        {
            return companyRepository.GetCarriers(search, appointedCarriers, currentUserCompanyId);
        }

        public object GetCompanyById(Guid companyId)
        {
            return companyRepository.GetCompanyById(companyId);
        }

        public Company CreateCarrierCompany(string companyName, string emailDomain)
        {
            companyRepository.CreateCarrierCompany(companyName, emailDomain);
            return GetByEmailDomain(emailDomain);
        }
    }
}
