import {defaultFormOptions, defaultOptions, jsonResponseHandler, standardCatch} from "./defaults";

export default class CarrierAppointmentService {
    static getCarrierAppointment(carrierCompanyId) {
        return fetch(`/api/carrier/${carrierCompanyId}/appointment`, defaultOptions).then(jsonResponseHandler);
    }

    static addCarrierAppointment(carrierCompanyId) {

        return fetch(`/api/carrier/${carrierCompanyId}/appointment`, {
            method:"post",
            ...defaultFormOptions
        }).then(jsonResponseHandler);
    }

    static updateCarrierAppointment(carrierCompanyId, brokerNotes) {
        const formData = new FormData();
        formData.append("notes", brokerNotes);

        return fetch(`/api/carrier/${carrierCompanyId}/appointment`, {
            method:"put",
            body: new URLSearchParams([...formData]),
            ...defaultFormOptions
        }).then(jsonResponseHandler);
    }

    static deleteAppointment(carrierCompanyId) {
        return fetch(`/api/carrier/${carrierCompanyId}/appointment`, {
            method:"delete",
            ...defaultOptions
        }).then(jsonResponseHandler).catch(standardCatch);
    }
}