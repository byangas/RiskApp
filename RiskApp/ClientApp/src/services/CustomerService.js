import React from "react";
import {defaultOptions, jsonResponseHandler, standardCatch} from "./defaults";


export default class CustomerService {
    static getCustomers() {
        return fetch('/api/broker/customer', defaultOptions).then(jsonResponseHandler);
    }

    static getCustomer(id) {
        return fetch('/api/broker/customer/' + id, defaultOptions).then(jsonResponseHandler);
    }

    static create(customer) {
        return fetch('/api/broker/customer', {
            method: "post",
            body: JSON.stringify(customer),
            ...defaultOptions
        }).then(jsonResponseHandler);
    }

    static delete(id) {
        return fetch('/api/broker/customer/' + id, {
            method: "delete",
            ...defaultOptions
        });

    }

    static update(customer) {

        return fetch('/api/broker/customer/' + customer.id, {
            method: "put",
            body: JSON.stringify(customer),
            ...defaultOptions
        }).then(jsonResponseHandler).catch(standardCatch);

    }
}