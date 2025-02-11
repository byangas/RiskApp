import {defaultFormOptions, defaultOptions, jsonResponseHandler, standardCatch} from "./defaults";



export default class CompanyService {
    static getCompanyByEmailDomain(emailDomain) {
        const url = `/api/company/?emailDomain=${emailDomain}`
        return fetch(url, defaultOptions).then(jsonResponseHandler).catch(standardCatch)
    }


    static getCompanyList(searchCriteria, filterByAppointedCarrier) {

        if(!searchCriteria) {
            searchCriteria = ""
        }
        if(!filterByAppointedCarrier) {
            filterByAppointedCarrier = false;
        }

        const url = `api/company/carrier/?search=${searchCriteria}&appointed=${filterByAppointedCarrier}`;

        return fetch(url, defaultOptions).then(jsonResponseHandler).catch(standardCatch)
    }

    static getCompanyById(companyId) {
        const url = `api/company/${companyId}`;

        return fetch(url, defaultOptions).then(jsonResponseHandler).catch(standardCatch)
    }

    static createCompany(companyName, emailDomain) {
        const url = `api/company/`;
        const form = new FormData();
        form.append("name", companyName)
        form.append("domain", emailDomain)


        return fetch(url,
            {
                method:"post",
                body:new URLSearchParams([...form]),
                ...defaultFormOptions
            } ).then(jsonResponseHandler).catch(standardCatch)
    }
}