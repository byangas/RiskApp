import React, {useState} from "react";
import {Button, Container} from "react-bootstrap";
import {useHistory, useParams} from "react-router-dom";
import PolicyService from "../../services/PolicyService";
import CustomerHeader from "./CustomerHeader";


export default function PolicyNew() {

    const {customerId} = useParams();
    const [policy, setPolicy] = useState({})
    const history = useHistory()


    function CreatePolicy() {
        if (!policy.description || !policy.description.length ) {
            alert("Please enter a policy description")
            return
        }

        //clean up. If the date is not set to a valid date, clear it up
        if (policy.renewalDate) {
            if (policy.renewalDate === '') {
                policy['renewalDate'] = undefined
            } else {
                policy.renewalDate = new Date(policy.renewalDate);
            }
        }

        PolicyService.create(policy, customerId).then((id) => {
            alert("Policy Created ");
            history.replace(`/broker/customer/${customerId}/policy/${id}/new`)
        })
    }


function setValue(field, value) {
    const policyTemp = {...policy}
    policyTemp[field] = value;
    setPolicy(policyTemp)
}


return (
    <Container>
        <CustomerHeader customerId={customerId}/>
        <h2>New Marketing Sheet</h2>
        <div>
            <label className="input-label" htmlFor="txtPolicyDescription">Describe the Policy you are marketing (for
                example "Cyber Insurance")</label>
            <input className="input-text" type="text" onChange={(e) => setValue('description', e.target.value)}/>
        </div>

        <div>
            <label className="input-label" htmlFor="txtPolicyDescription">Policy Expiration Date (Not Required)</label>
            <input className="input-text" type="date" onChange={(e) => setValue('renewalDate', e.target.value)}/>
        </div>

        <div>
            <Button onClick={() => CreatePolicy()}>Next</Button>
        </div>
    </Container>
)
}