import React, { useEffect, useState } from "react";
import Container from "react-bootstrap/Container";
import {useHistory, useParams} from "react-router-dom";
import CustomerService from "../../services/CustomerService";
import Button from "react-bootstrap/Button";
import CustomerEdit from "./CustomerEdit";
import PolicyService from "../../services/PolicyService";
import PolicyTile from "./PolicyTile";

export default function CustomerDetail() {
    const { customerId } = useParams()
    const history = useHistory()
    const [customer, setCustomer] = useState({})
    const [showEdit, setShowEdit] = useState(false);
    const [policies, setPolicies] = useState()

    function closeEditor() {
        if(!customerId) {
            history.goBack();
        }
        else {
            setShowEdit(false)
        }
    }

    function LoadPolicies() {
        PolicyService.getPolicies(customerId).then(policies => {
            const policyElements = policies.map((policy) => {
                return <PolicyTile key={policy.id} policy={policy} clickHandler={policyClicked}/>
            });
            setPolicies(policyElements)
        })
    }

    function policyClicked(policy) {
        history.push(`/broker/customer/${customerId}/policy/${policy.id}`)
    }

    function newMarketingSheet() {
        history.push(`/broker/customer/${customerId}/policy/new`);
    }

    useEffect(() => {
        if(customerId) {
            LoadPolicies();
        }
    }, [])


    useEffect(() => {
        if (customerId) {
            CustomerService.getCustomer(customerId).then(acct => {
                setShowEdit(false)
                setCustomer(acct);
            })
        } else {
            setShowEdit(true)
        }
    }, [customerId])


    return (
        <Container>
            {showEdit &&
                <CustomerEdit customer={customer} closeEditor={closeEditor} />
            }
            {!showEdit &&
                <>
                    <div>
                        <h2> {customer.firmName}</h2>
                    </div>
                    <Button onClick={() => setShowEdit(true)}>View</Button>
                </>
            }

            {customerId && !showEdit &&
                <>
                    <div className='section-header'>Marketing Worksheets</div>
                    <Button onClick={newMarketingSheet}>Create new Marketing Worksheet</Button>
                    <div> {policies} </div>
                </>
            }


        </Container>
    )
}


