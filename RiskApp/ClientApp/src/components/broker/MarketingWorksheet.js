import React, {useState} from "react";
import {useHistory} from "react-router-dom";
import PolicyService from "../../services/PolicyService";
import {MarketingContact} from "./MarketingContact";
import Button from "react-bootstrap/Button";

export function MarketingWorksheet({policyId, customerId}) {

    const [marketingSheet, setMarketingSheet] = useState([])
    const history = useHistory()

    const LoadMarketingSheet = () => {
        if (!policyId) {
            return;
        }
        PolicyService.getMarketingSheet(policyId).then(marketingSheet => {
            const contacts = marketingSheet.map(contact => <MarketingContact deleteContact={deleteMarketingContact}
                                                                             contact={contact} key={contact.id}/>);
            setMarketingSheet(contacts)
        })
    }

    function deleteMarketingContact(marketingContact) {
        PolicyService.deleteMarketingContact(marketingContact.id).then(() => {
            alert("removed contact " + marketingContact.name)
            LoadMarketingSheet();
        })
    }

    return <>
        <div className="section-header">Marketing Worksheet</div>
        {marketingSheet}
    </>;

}