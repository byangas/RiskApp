import React, {useState} from "react";
import BrokerContacts from "./contact/BrokerContacts";
import Button from "react-bootstrap/Button";
import {Container} from "react-bootstrap";
import CompanyList from "./CompanyList";
import {useHistory} from "react-router-dom";

export default function ContactManagement() {

    const [showCompanies, setShowCompanies] = useState(false)
    const history = useHistory()

    const createContact = <Button primary onClick={() => history.push("/broker/contacts/add")}>Create Contact</Button>
    return (
        <Container>
            {!showCompanies &&
            <>
                <Button onClick={() => setShowCompanies(true)}>View Companies</Button>
                &nbsp;{createContact}
                <BrokerContacts/>
            </>

            }
            {showCompanies &&
            <>
                <Button onClick={() => setShowCompanies(false)}>View Contacts</Button>
                &nbsp;{createContact}
                <CompanyList/>
            </>

            }


        </Container>
    )
}