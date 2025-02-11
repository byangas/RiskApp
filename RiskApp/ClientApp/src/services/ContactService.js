// temporary for demo purposes. This will be replaced when the API returns the full set of data
import CompanyService from "./CompanyService";
import {defaultOptions, jsonResponseHandler, standardCatch} from "./defaults";


export default class ContactService {
    static getContacts(showMyCompanyOnly, searchText, sortBy, filterByCompanyId) {
        if(!searchText) {
            searchText = "";
        }
        if(!filterByCompanyId) {
            filterByCompanyId = ""
        }
        const uri = `/api/contact/carrier/?companyId=${filterByCompanyId}&showMyCompanyOnly=${showMyCompanyOnly}&search=${searchText}&sortBy=${sortBy}`
        return fetch(uri, defaultOptions).then(jsonResponseHandler).catch(standardCatch)
    }

    static getContact(id) {
        const uri = '/api/contact/?contactId=' + id;
        return fetch(uri, defaultOptions).then(jsonResponseHandler).catch(standardCatch);
    }

    static getContactByEmail(email) {
        const uri = '/api/contact/?email=' + encodeURI(email)
       return fetch(uri, defaultOptions).then(jsonResponseHandler).catch(standardCatch)
    }
    static  validateEmail(email) {
        // NOTE: This is all prototype code. Final implementation should move all this logic
        /// to server side
        const exampleResponse = {
            profile: null,
            company: null,
        }

        let emailSplit = email.split("@");
        const emailDomain = emailSplit[1];
        // invalid email address
        if (emailSplit.length != 2) {
            return Promise.resolve(exampleResponse);
        }
        //todo: get contact by email
        return this.getContactByEmail(email).then(contact=> {
            if(contact) {
                exampleResponse.profile = contact
                return Promise.resolve(exampleResponse)
            }
            else {
                // if no existing contact found, see if company domain is valid
                return CompanyService.getCompanyByEmailDomain(emailDomain).then(company => {
                    exampleResponse.company = company;
                    return Promise.resolve(exampleResponse)
                })
            }
        })
    }

    static updateContact(contact) {
        return fetch('/api/contact/' + contact.id, {
            method: 'put',
            body: JSON.stringify(contact),
            ...defaultOptions
        })
    }
    static createContact(contact) {


        return fetch('/api/contact', {
            method: 'post',
            body: JSON.stringify(contact),
            ...defaultOptions
        }).then(jsonResponseHandler)
    }

    static addCompanyContact(id) {
        return fetch('/api/contact/' + id, {
            method: 'post',
            ...defaultOptions
        })
    }

    static removeCompanyContact(id) {
        return fetch('/api/contact/' + id, {
            method: 'delete',
            ...defaultOptions
        })
    }

}