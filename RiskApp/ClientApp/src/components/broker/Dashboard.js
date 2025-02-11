import React, {useEffect, useState} from "react";
import {Container} from "react-bootstrap";
import Spinner from '../../components/controls/Spinner'
import PolicyService from "../../services/PolicyService";
import {useHistory} from "react-router-dom";

export default function Dashboard() {
    const [policySummary, setPolicySummary] = useState()
    const history = useHistory()
    const [sortBy, setSortBy] = useState("renewal")

    async function LoadPolicies() {
        const policies = await PolicyService.policySummaries(sortBy);
        setPolicySummary(policies)
    }

    //init
    useEffect(() => {
        LoadPolicies();
    }, [])

    useEffect(() => {
        LoadPolicies();
    }, [sortBy])
    if (!policySummary) {
        return (
            <Container>
                <Spinner/>
            </Container>
        )
    }

    function goToPolicy(policy) {
        history.push(`/broker/customer/${policy.customerId}/policy/${policy.policyId}/`)
    }
    const summaryViews = policySummary.map((policy) => {
        const policyCreatedDate = new Date(policy.createdDate)
        let renewalDate = null;
        return (
            <div onClick={()=>goToPolicy(policy)} key={policy.policyId} className="policy-summary-tile clickable">
                {policy.customer} - {policy.description}
                <div>
                    <label className="text-info">Markets: </label> {policy.marketingSheetCount}&nbsp;&nbsp;
                    { policy.renewalDate && (renewalDate = new Date(policy.renewalDate)) && (
                        <>
                            <label className="text-info">Renewal Date: </label> {renewalDate.toLocaleDateString("medium")}
                        </>
                    )}

                </div>
                <div><label className="text-info">Created by: </label> {policy.brokerName} on {policyCreatedDate.toLocaleDateString("medium")}</div>
            </div>
        )
    })

    return (

        <Container>
            <div className="section-header">Active Marketing Sheets</div>
            <div style={{marginBottom:"9px"}}>
                Order By <select value={sortBy} onChange={(e)=> setSortBy(e.target.value)}>
                <option value="created">Created Date</option>
                <option value="renewal">Renewal Date</option>
            </select>
            </div>
            <div>
                {summaryViews}
            </div>
        </Container>
    )
}