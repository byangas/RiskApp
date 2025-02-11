import {defaultFormOptions, defaultOptions, jsonResponseHandler, standardCatch} from "./defaults";

export default class PolicyService {
    static getInsuranceTypes(policy) {
        const insurancetypes = [];
        if (policy.insurance) {
            if (policy.insurance.workersComp) {
                insurancetypes.push("workersComp");
            }
            if (policy.insurance.commercialAuto) {
                insurancetypes.push("commercialAuto");
            }
            if (policy.insurance.commercialProperty) {
                insurancetypes.push("commercialProperty");
            }
            if (policy.insurance.generalLiability) {
                insurancetypes.push("generalLiability");
            }
            if (policy.insurance.specialty) {
                insurancetypes.push("specialty");
            }
        }
        return insurancetypes;
    }

    static create(policy, customerId) {

        return fetch(`/api/policy/customer/${customerId}/`, {
            method: "post",
            body: JSON.stringify(policy),
            ...defaultOptions
        }).then(jsonResponseHandler)
    }

    static getPolicies(customerId) {
        return fetch(`/api/broker/customer/${customerId}/policy`, {
            ...defaultOptions
        }).then(jsonResponseHandler)
    }

    static getPolicy(policyId) {
        return fetch(`/api/policy/${policyId}`, {
            ...defaultOptions
        }).then(jsonResponseHandler)
    }

// not really "policy". Really just the appetite form
    static updateAppetite(policyId, policy) {
        const insuranceTypes = PolicyService.getInsuranceTypes(policy);

        return fetch(`/api/policy/${policyId}/appetite`, {
            method: "put",
            body: JSON.stringify({policy: JSON.stringify(policy), insuranceTypes},),
            ...defaultOptions
        }).then(jsonResponseHandler);
    }

    static addMarketingContact(policyId, contactId) {
        return fetch(`/api/policy/${policyId}/contact/create/${contactId}`, {
            method: "put",
            ...defaultOptions
        }).then(jsonResponseHandler);
    }

    static getMarketingSheet(policyId) {
        return fetch(`/api/policy/${policyId}/marketingsheet`, {
            ...defaultOptions
        }).then(jsonResponseHandler);
    }

    static getMarketingSheetContact(marketingSheetContactId) {
        return fetch(`/api/policy/marketingsheet/${marketingSheetContactId}`, {
            ...defaultOptions
        }).then(jsonResponseHandler);
    }


    static deleteMarketingContact(marketingSheetContactId) {

        return fetch(`/api/policy/marketingsheet/${marketingSheetContactId}`, {
            method: "delete",
            ...defaultOptions
        }).then(jsonResponseHandler);

    }
    static updateMarketingSheetContactStatus(marketingSheetContactId, status) {
        const formData = new FormData();
        formData.append("status", status);
        return fetch(`/api/policy/marketingsheet/${marketingSheetContactId}/status`, {
            method: "put",
            body: new URLSearchParams([ ...formData]),
            ...defaultFormOptions
        }).then(jsonResponseHandler)
            .catch(standardCatch);
    }

    static addPolicyNote(policyId, newNoteContent) {
        const formData = new FormData();
        formData.append("note", newNoteContent);
        return fetch(`/api/policy/${policyId}/note`, {
            method: "post",
            body: new URLSearchParams([ ...formData]),
            ...defaultFormOptions
        }).then(jsonResponseHandler)
            .catch(standardCatch);
    }

    static addMarketingContactNote(marketingSheetContactId, newNoteContent) {
        const formData = new FormData();
        formData.append("note", newNoteContent);
        return fetch(`/api/policy/marketingsheet/${marketingSheetContactId}/note`, {
            method: "put",
            body: new URLSearchParams([ ...formData]),
            ...defaultFormOptions
        }).then(jsonResponseHandler)
            .catch(standardCatch);
    }

    static getMarketingSheetNotes(marketingSheetContactId) {
        return fetch(`/api/policy/marketingsheet/${marketingSheetContactId}/note`, {
            ...defaultOptions
        }).then(jsonResponseHandler)
            .catch(standardCatch);
    }

    static updateMarketingSheetContactField(marketingSheetContactId, field, value) {
        const formData = new FormData();
        formData.append(field, value);
        return fetch(`/api/policy/marketingsheet/${marketingSheetContactId}/${field}`, {
            method: "put",
            body: new URLSearchParams([ ...formData]),
            ...defaultFormOptions
        }).then(jsonResponseHandler)
            .catch(standardCatch);
    }

    static sendAppetiteFitRequest(marketingSheetContactId, message) {
        const formData = new FormData();
        formData.append("message", message);
        return fetch(`/api/policy/marketingsheet/${marketingSheetContactId}/afr`, {
            method: "post",
            body: new URLSearchParams([ ...formData]),
            ...defaultFormOptions
        }).then(jsonResponseHandler)
            .catch(standardCatch);
    }

    static deleteAppetite(policyId) {
        return fetch(`/api/policy/${policyId}/appetite`, {
            method: "delete",
            ...defaultOptions
        }).then(jsonResponseHandler);
    }

    static async policySummaries(sortBy) {

        const response = await fetch(`/api/policy/?orderBy=${sortBy}`, {
            ...defaultOptions
        });
        return jsonResponseHandler(response)
    }

    static updatePolicyDetails(policyId, description, renewalDate) {
        const formData = new FormData();
        formData.append("description", description);
        if(renewalDate) {
            formData.append("renewalDate", renewalDate);
        }

        return fetch(`/api/policy/${policyId}/details`, {
            method: "put",
            body: new URLSearchParams([ ...formData]),
            ...defaultFormOptions
        }).then(jsonResponseHandler)
            .catch(standardCatch);
    }

    static deletePolicy(policyId) {
        return fetch(`/api/policy/${policyId}`, {
            method: "delete",
            ...defaultOptions
        }).then(jsonResponseHandler);
    }

    static getPolicyNotes(policyId) {
        return fetch(`/api/policy/${policyId}/note`, {
            method: "get",
            ...defaultOptions
        }).then(jsonResponseHandler);
    }
}