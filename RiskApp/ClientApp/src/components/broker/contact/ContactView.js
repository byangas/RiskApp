import React, {useEffect, useState} from "react";
import {useParams} from "react-router-dom";
import ContactService from "../../../services/ContactService";
import {Container, Spinner} from "react-bootstrap";
import ContactDetail from "./ContactDetail";

export default function ContactView() {
    const {contactId} = useParams();
    const [contact, setContact] = useState()

    useEffect(() => {
        ContactService.getContact(contactId).then(contact => {

            setContact(contact)
        })
            .catch((error)=> {
                console.log(error)
                alert("Error loading contact information")
            }
            )
    }, [])

    if (!contact) {
        return <Spinner/>
    }

    return (
        <Container>
            <ContactDetail contact={contact} />
        </Container>
    )
}