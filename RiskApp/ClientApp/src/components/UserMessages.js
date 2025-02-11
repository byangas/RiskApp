import React, {useContext, useEffect, useState} from "react";
import UserContext from "../UserContext";
import {Button, Container} from "react-bootstrap";
import MessageService from "../services/MessageService";

export default function UserMessages() {
    const [messages, setMessages] = useState([]);

    useEffect(() => {
        MessageService.getUnreadMessages().then(myMessages => {
            setMessages(myMessages)
        })

    }, [])

    function removeMessage(message) {

        const remainingMessages = messages.filter(function(item) {
            return item !== message;
        })
        setMessages(remainingMessages);
    }

    if(!messages || !messages.length) {
        return (
            <Container>
                <div className="section-header">Messages</div>
                <div className="totally-centered" >
                    <div style={{border:"1px solid white", padding:"20px", borderRadius:"6px"}}>
                        You have no unread messages at this time. <br/>
                        Messages from Brokers or Carriers would appear here when they send a message or Appetite Check to you
                    </div>

                </div>
            </Container>
        )
    }

    return (
        <Container>
            <div className="section-header">Messages</div>
            {messages.map((value, index) => <Message key={value.message.id} message={value} removeMessage={removeMessage}></Message>)}
        </Container>

    )
}

function Message(props) {
    const {message: messageEnvelope, removeMessage} = props;
    const message = messageEnvelope.message;
    const [replyContent, setReplyContent] = useState()
    const [showReply, setShowReply] = useState(false)
    let messageTitle = "Message";
    let appetiteFitViewUrl = `/appetitefitrequest/${message.id}/view`
    const isAppetiteFitRequest = message.messageType === "AppetiteFitRequest";
    if (isAppetiteFitRequest) {
        messageTitle = "Appetite Fit Request"
    }

    function markAsRead() {
        MessageService.markAsRead(message.id).then(() =>{
            removeMessage(messageEnvelope)
        })
    }

    function replyToMessage() {
        if(!replyContent || replyContent.length === 0) {
            alert("Please fill in response")
        }
        MessageService.reply(message.id,replyContent ).then(() =>{
            removeMessage(messageEnvelope)
        })
    }


    return (
        <div className="tile tile-no-hover">
            <div className="tileContent">
                <div className="tile-title">{messageTitle}</div>
                <div className="text-info">{messageEnvelope.subject}</div>
                <div style={{margin: "9px"}}>
                    {message.messageContent}

                </div>
                {isAppetiteFitRequest &&
                    <a href={appetiteFitViewUrl}>View Risk Details</a>
                }

                <div className="message-from">
                    {messageEnvelope.sentBy}
                </div>
                {!showReply && !isAppetiteFitRequest && (
                    <div>
                        <Button className="btn-inline" onClick={()=> setShowReply(true)}>Reply</Button>&nbsp;

                        <Button className="btn-inline"  onClick={()=> markAsRead(messageEnvelope)}>Mark As Read</Button>
                    </div>
                )}

                {showReply &&
                    <div>
                        <div style={{padding: "6px", textAlign: "left"}}>
                            <label className="input-label">Reply</label>
                            <textarea value={replyContent} onChange={(e)=>{setReplyContent(e.target.value)} }className="input-text-area"></textarea>
                        </div>
                        <div>
                            <Button onClick={()=> replyToMessage()} className="btn-inline">Send</Button>
                            <Button className="btn-inline"  onClick={()=> setShowReply(false)}>Cancel</Button>
                        </div>
                    </div>

                }
            </div>


        </div>

    )
}