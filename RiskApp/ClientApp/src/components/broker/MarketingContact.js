import React, {useEffect, useRef, useState} from "react";
import Button from "react-bootstrap/Button";
import {decimalFromString, formatNumber, numberFromString} from "../utils/numberUtils";
import PolicyService from "../../services/PolicyService";
import MessageService from "../../services/MessageService";
import {standardCatch} from "../../services/defaults";
import debounce from "../utils/debounce";

const updateMarketingSheetFieldDebounced = debounce((marketingContactId, field, value, callback) => {

    PolicyService.updateMarketingSheetContactField(marketingContactId, field, value).then(callback());

}, 4000)

export function MarketingContact(props) {

    const [viewDetail, setViewDetail] = useState(false)
    const [premium, setPremium] = useState(props.contact.premium)
    const [commission, setCommision] = useState(props.contact.commission)
    const marketingContact = props.contact
    const [needStatusUpdate, setNeedsStatusUpdate] = useState(false)
    const [status, setStatus] = useState(marketingContact.status)
    const statusDescription = {
        new: "New",
        pendingMfr: "Waiting Appetite Response",
        noFit: "No Market Fit",
        quote: "Needs Quote",
        pendingQuote: "Waiting for Quote Response",
        received: "Quote Received",
        blocked: "Blocked"
    }

    useEffect(() => {
        if (needStatusUpdate)
            PolicyService.updateMarketingSheetContactStatus(marketingContact.id, status)

        setNeedsStatusUpdate(true)
    }, [status])

    function getStatusDescription(status) {
        return statusDescription[status]
    }

    if (viewDetail) {
        return (<MarketingContactDetail />)
    } else {
        return (<MarketingContactSummary viewDetails={viewDetail} />)
    }

    function MarketingContactSummary({viewDetails}) {

        let summary;
        let styleForTitle = {margin: "-2px", borderRadius: "3px"}


        if (status === "received" && !viewDetails) {
            summary = (
                <div>
                    Premium: ${formatNumber(premium)} Commission: ${formatNumber(commission)}
                </div>
            )
            styleForTitle = null;
        }

        return (
            <div onClick={() => setViewDetail(true)} className="tile" style={{width: "100%"}}>
                <div className="tileContent" style={{textAlign: "left"}}>
                    <div className="tile-title " style={styleForTitle}>
                        <span>{marketingContact.companyName} - {marketingContact.name} </span>
                        <span>{getStatusDescription(status)} <Button className="btn-danger, btn-inline">View</Button> </span>

                    </div>
                    {summary}
                </div>
            </div>
        )
    }



    function MarketingContactDetail() {

        const [marketingContactNotes, setMarketingContactNotes] = useState([])
        const [messages, setMessages] = useState([])
        const [newNoteContent, setNewNoteContent] = useState("")
        const [messageContent, setMessageContent] = useState("")
        const premiumRef = useRef();
        const commissionRef = useRef();

        useEffect(() => {
            if(premiumRef.current) {
                premiumRef.current.value = formatNumber(premium);
            }
            if(commissionRef.current) {
                commissionRef.current.value = formatNumber(commission);
            }

            loadNotes()
            loadMessages()
        }, [])

        function updatePremium(e) {
            const temp_premium = numberFromString(e.target.value);

            premiumRef.current.value = formatNumber(temp_premium);
            updateMarketingSheetFieldDebounced(marketingContact.id,"premium", temp_premium, ()=>{
                setPremium(temp_premium)
            })
        }

        function updateCommission(e) {
            const temp_commission = numberFromString(e.target.value);

            commissionRef.current.value = formatNumber(temp_commission);
            updateMarketingSheetFieldDebounced(marketingContact.id, "commission", temp_commission, ()=>{
                setCommision(temp_commission)
            })
        }

        function loadNotes() {
            PolicyService.getMarketingSheetNotes(marketingContact.id).then(notes => {
                setMarketingContactNotes(notes)
            })
        }

        function loadMessages() {
            MessageService.getMessageThread(marketingContact.id).then(ms => {
                setMessages(ms)
            })
        }

        function addNewNote() {
            PolicyService.addMarketingContactNote(marketingContact.id, newNoteContent).then(() => {
                setNewNoteContent("")
                loadNotes()
            })
        }

        function SendMessage() {
            MessageService.send(marketingContact.profileId, marketingContact.id, messageContent).then(
                ()=> {
                    setMessageContent("");
                    loadMessages();
                }
            ).catch(standardCatch)
        }

        return (
            <div className="tile tile-no-hover" style={{width: "100%"}}>
                <div className="tileContent" style={{textAlign: "left"}}>

                    <div className="tile-title" >
                        <span style={{width:"100%"}} onClick={() => setViewDetail(false)}>{marketingContact.companyName} - {marketingContact.name}</span>
                        <span onClick={()=>{ return false}}>
                           Change Status <select onChange={(e) => setStatus(e.target.value)}
                                                 value={status}>
                                <option value="new">New</option>
                                <option value="pendingMfr">Waiting Appetite Response</option>
                                <option value="noFit">No Market Fit</option>
                                <option value="quote">Needs Quote</option>
                                <option value="pendingQuote">Waiting for Quote Response</option>
                                <option value="received">Quote Received</option>
                                <option value="blocked">Blocked</option>
                            </select>
                            <Button onClick={() => setViewDetail(false)}
                                    className="btn-danger, btn-inline">Close</Button>
                        </span>

                    </div>

                    {status === "received" &&
                        <div>
                            <div className="section-header">Quote</div>
                            <div>
                                Premium <input ref={premiumRef} className="input-text" onChange={updatePremium} />
                                Commission <input ref={commissionRef} onChange={updateCommission} className="input-text"/>
                            </div>
                        </div>
                    }

                    <p></p>
                    <div style={{display: "flex", flexDirection: "row"}}>
                        <div className="tileContent">
                            <div>Phone <a href="tel:">{marketingContact.phone}</a></div>
                            <div>Email <a href="mailto:">{marketingContact.email}</a></div>
                            { marketingContact.mobilePhone && (
                                <div>Mobile <a href={`tel:${marketingContact.mobilePhone}`}>{marketingContact.mobilePhone}</a></div>
                            )}
                        </div>
                        <div>
                            <img style={{margin: "3px", maxWidth: "100px"}}
                                 src={`/assets/companylogo/${marketingContact.logo}`}
                                 height="45"/>
                        </div>
                    </div>

                    <div>
                        <div className="section-header">Notes</div>
                        <NotesDisplay notes={marketingContactNotes}/>
                        <span>Add Note</span>
                        <textarea className="marketing-contact-notes" className="input-text-area" lines="3"
                                  value={newNoteContent}
                                  onChange={(e) => setNewNoteContent(e.target.value)}> </textarea>
                        <Button onClick={() => addNewNote()}>Add Note</Button>
                    </div>

                    <div>
                        <div className="section-header">Messages</div>
                        <MessageDisplay messages={messages}
                                        marketingContactOwnerId={marketingContact.createdByProfileId}/>
                        <span>New message</span>
                        <textarea value={messageContent} onChange={(e)=>{setMessageContent(e.target.value)}} className="input-text-area" lines="3"> </textarea>
                        <Button onClick={()=>{SendMessage()}}>Send Message</Button>
                    </div>


                    <p style={{textAlign: "right"}}>
                        <Button onClick={() => props.deleteContact(marketingContact)}>Delete</Button>
                    </p>

                </div>
            </div>
        )
    }

    function NotesDisplay(props) {
        const {notes} = props;
        if (!notes) {
            return null;
        }

        return notes.map((note, index) => {
            const createdDate = new Date(note.created);
            return (
                <div key={index}>
                    <div className="marketing-contact-notes">
                        {note.note}
                        <div style={{fontSize: "9px"}}>{ createdDate.toLocaleString("medium") }</div>
                    </div>

                </div>
            )
        })
    }

    function MessageDisplay(props) {
        const {messages, marketingContactOwnerId} = props;

        if (!messages) return null;

        return messages.map((message, index) => {
            let className = "marketing-contact-message"
            let typeDescription = "Sent"

            if (message.senderProfileId !== marketingContactOwnerId) {
                className = "marketing-contact-message-received"
                typeDescription = "Received"
            }

            const messageSentDate = new Date(message.createdDate);
            let messageContent = message.messageContent
            let title = null;
            if (message.messageType === "AppetiteFitRequest") {
                title = 'Appetite Fit Request'
            }
            else if(message.messageType === "AppetiteFitResponse") {
                title = 'Appetite Fit Response'
            }
            const readDate = new Date(message.readDate)
            return (
                <div key={message.id}>
                    <div className={className}>
                        {title &&
                            <div className='marketing-contact-message-title' ><b>{title}</b></div>
                        }
                        {messageContent}
                        <div
                            style={{fontSize: "10px"}}>{typeDescription}: {messageSentDate.toLocaleString("medium")}</div>
                        {message.readDate &&
                            <div style={{fontSize: "10px"}}>Read at: {readDate.toLocaleString("medium")}</div>
                        }

                    </div>
                </div>
            )
        })
    }
}