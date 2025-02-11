import React, {useEffect, useState} from 'react';
import Button from 'react-bootstrap/Button'
import PolicyService from "../../services/PolicyService";
import {useHistory, useParams} from "react-router-dom";
import {Container, Modal} from "react-bootstrap";
import {MarketingContact} from "./MarketingContact";
import {AppetiteFitRequestForm} from "./PolicyTile";
import PolicyNotes from "./PolicyNotes";
import CustomerHeader from "./CustomerHeader";

export default function PolicyEdit() {
    const {policyId, customerId, state} = useParams()
    const [showPolicyEditor, setShowPolicyEditor] = useState(false)
    const [policy, setPolicy] = useState({});
    const history = useHistory();
    const [showAppetiteFitRequestInput, setShowAppetiteFitRequestInput] = useState(false)
    const [messageContent, setMessageContent] = useState("")
    const [marketingSheet, setMarketingSheet] = useState([])

    useEffect(() => {
        loadMarketingSheet();
        loadPolicy();
    }, []);

    function loadMarketingSheet() {
        setMarketingSheet([])
        PolicyService.getMarketingSheet(policyId).then(marketingSheetReceived => {

            // if this is a new policy/marketing sheet, then we just launch the user straight into adding contacts
            // to marketing sheet
            if (marketingSheetReceived.length === 0 && state === "new") {
                addContactsToMarketingSheet();
                return;
            }
            setMarketingSheet(marketingSheetReceived)
        })
    }

    function addContactsToMarketingSheet() {
        history.push(`/broker/customer/${customerId}/policy/${policyId}/marketing`)
    }

    function loadPolicy() {
        PolicyService.getPolicy(policyId).then(policy => {
            setPolicy(policy)
        });
    }

    function deleteMarketingContact(marketingContact) {
        PolicyService.deleteMarketingContact(marketingContact.id).then(() => {
            alert("Removed contact " + marketingContact.name)
            loadMarketingSheet();
        })
    }

    function savePolicy() {
        PolicyService.updatePolicyDetails(policy.id, policy.description, policy.renewalDate).then(()=>{
            loadPolicy()
            setShowPolicyEditor(false)
            alert("Updated")
        }).catch((error)=> alert("Error saving"))
    }
    function setValue(field, value) {
        const policyTemp = {...policy}
        policyTemp[field] = value;
        setPolicy(policyTemp)
    }

    function showAppetite() {
        history.push(`/broker/customer/${customerId}/policy/${policyId}/appetite`)
    }

    function deletePolicy() {
        PolicyService.deletePolicy(policyId).then(()=> {
            alert("Policy Deleted")
            history.goBack();
        })
    }

    function sendAppetiteFitRequest() {
        if(!messageContent || messageContent.trim().length === 0) {
            alert("Message is required")
            return;
        }
        //get list of contacts to send appetite check to
        const recipientsList = [];
        marketingSheet.forEach(contact => {
            if(contact.sendAppetite) {
                recipientsList.push(contact)
            }
        })

        let sendCount = recipientsList.length;
        recipientsList.forEach((contact => {
            PolicyService.sendAppetiteFitRequest(contact.id, messageContent);
            sendCount = sendCount - 1;
            if(sendCount === 0) {
                setTimeout(()=> {
                        loadMarketingSheet();
                    }, 4000
                )
            }
        }))
        setShowAppetiteFitRequestInput(false)
    }
    const selectedStyle={
        background:"gray"
    }
    const notSelectedStyle = {

    }

    let policyRenewalDate = null
    //clean up the date format and strip out the time information
    if (policy.renewalDate) {
        const clean = new Date(policy.renewalDate);
        // so that we can assign the value to the native "date" functionality in the htm; input
        // it requires format of yyyy-mm-dd'
        policyRenewalDate = clean.toISOString().slice(0, 10);
    }

    if(showPolicyEditor) {

        return (
            <Container>
                <CustomerHeader customerId={customerId}/>
                <div className="section-header">Policy Info</div>

                <div>
                    <label className="input-label" htmlFor="txtPolicyDescription">Describe the Policy you are marketing (for
                        example "Cyber Insurance")</label>
                    <input className="input-text" id="txtPolicyDescription" value={policy.description} type="text"
                           onChange={(e) => setValue('description', e.target.value)}/>
                </div>
                <div>
                    <label className="input-label" htmlFor="policyRenewalDate">Policy Expiration Date (Not
                        Required)</label>
                    <input className="input-text" value={policyRenewalDate} type="date" id="policyRenewalDate"
                           onChange={(e) => setValue('renewalDate', e.target.value)}/>
                </div>
                <div>
                    <Button onClick={()=> savePolicy()}>Save</Button>&nbsp;<Button variant="secondary" onClick={() => {
                    loadPolicy()
                    setShowPolicyEditor(false);
                } }>Cancel</Button>
                </div>
            </Container>

        )
    }

    return (
        <Container>
            <Modal show={showAppetiteFitRequestInput}>
                <Modal.Body>
                    <div>
                        Customize message in Appetite Check
                    </div>
                    <label className="input-label">Message</label>
                    <textarea placeholder="Enter Additional Information or Notes" className="input-text-area"
                              lines="3"
                              value={messageContent} onChange={(e) => {
                        setMessageContent(e.target.value)
                    }}/>
                    <p></p>
                    <div>Select contacts to send Appetite check to:</div>
                    <div>
                        {
                            marketingSheet.map((contact) => {
                                const style = contact.sendAppetite ? selectedStyle : notSelectedStyle;

                                return (
                                    <div style={style} onClick={(e) => {
                                        contact.sendAppetite = !contact.sendAppetite;
                                        setMarketingSheet([...marketingSheet]);
                                    }
                                    }
                                         className="marketing-contact-add clickable">{contact.companyName} -{contact.name}</div>
                                )
                            })
                        }
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <Button onClick={sendAppetiteFitRequest}>Send</Button>&nbsp;
                    <Button variant="secondary" onClick={()=> setShowAppetiteFitRequestInput(false)}>Cancel</Button>
                </Modal.Footer>
            </Modal>

            <CustomerHeader customerId={customerId}/>
            <div className="section-header">Policy Info</div>
            <div>
                {policy.description}
            </div>
            {policy.renewalDate && (
              <div>
                 <label className="text-info">Renewal Date: </label> {new Date(policy.renewalDate).toLocaleDateString("medium")}
              </div>
            )}
            {!policy.renewalDate && (
                <div>No Renewal Date</div>
            )}
            <Button onClick={()=> setShowPolicyEditor(true)}>Edit policy description and renewal date</Button>&nbsp;
            <p>&nbsp;</p>

            <PolicyNotes policyId={policyId}/>
            <div>
                <div className="section-header">Marketing Worksheet</div>
            </div>
            {
                marketingSheet.map(contact => <MarketingContact deleteContact={deleteMarketingContact} contact={contact} key={contact.id}/>)
            }
            <Button onClick={addContactsToMarketingSheet}>Add Contacts to Marketing Sheet</Button>

            {marketingSheet.length > 0 && !policy.detail && (
                <>
                    <p>&nbsp;</p>
                    <p>&nbsp;</p>
                    <div className="section-header">Appetite Fit Request</div>
                    <div>Fill out form to send to Carriers to verify that they have an appetite for this policy before
                        creating quotes
                    </div>
                    <Button onClick={showAppetite}>Create Appetite Fit Request</Button>
                </>
            )}

            {marketingSheet.length > 0 && policy.detail && (
                <>
                    <p>&nbsp;</p>
                    <div className="section-header">Appetite Fit Request</div>
                    <div className="tile">

                        <div className="tileContent">
                            <div className="insurance-title">Policy</div>
                            <AppetiteFitRequestForm insurance={policy.detail.insurance}/>
                            <p></p>
                            <div>

                                <Button onClick={() => setShowAppetiteFitRequestInput(true)}> <img
                                    className="btn-icon" src="/assets/icons/message.svg"/>&nbsp; Send Appetite Check</Button>&nbsp;
                                <Button onClick={showAppetite}>Edit Policy</Button>
                            </div>
                        </div>
                    </div>

                </>
            )}


            <p>&nbsp;</p>
            <div>This will delete the Marketing Sheet, Appetite and all related information.</div>
            <div style={{marginTop: "18px"}}>
                <Button variant="danger" onClick={()=> deletePolicy()} >Delete Policy/Marketing Sheet</Button>
            </div>


        </Container>

    );


}

