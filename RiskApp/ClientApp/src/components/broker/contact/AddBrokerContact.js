import React, {useState} from "react";
import {Container} from "react-bootstrap";
import ContactService from "../../../services/ContactService";
import Button from "react-bootstrap/Button";
import CompanyTile from "../CompanyTile";
import CompanyService from "../../../services/CompanyService";
import ContactDetail from "./ContactDetail";
import ProfileEdit from "../../ProfileEdit";

export default function AddBrokerContact() {
    const [invalidEmail, setInvalidEmail] = useState(false)
    const [emailDomain, setEmailDomain] = useState();
    // multi step process. First step is to get email address, and then go from there.
    const [needsEmailInput, setNeedsEmailInput] = useState(true)
    const [contact, setContact] = useState({})

    const setValue = (name, value) => {
        //clone contact
        const temp = {...contact}
        temp[name] = value;
        setContact(temp);
    }

    function editComplete() {
        setContact({})
        setNeedsEmailInput(true)
    }

    function createCompany() {
        CompanyService.createCompany(contact.companyName, emailDomain).then(() => {
            //should run through the normal process now that company is created
            validateEmail()
        })
    }

    function validateEmail() {
        // todo: validate email with regular expression to make sure format is at least correct (minimal)
        let emailSplit = contact.email.split("@");
        const emailDomainTemp = emailSplit[1];
        // invalid email address
        if (emailSplit.length != 2 || !emailDomainTemp.length) {
            return;
        }
        setEmailDomain(emailDomainTemp)

        //checks to see if email has correct domain name
        // and if the contact exists
        ContactService.validateEmail(contact.email).then(response => {
            // didn't find company or profile, so must be invalid email
            // either because it was entered in correctly, or company email domain doesn't exist
            const temp = {...contact, ...response}

            // setInvalidEmail(!response.profile && !response.company)
            setInvalidEmail(false)
            setNeedsEmailInput(false)
            setContact(temp)
        })
    }

    return (
        <Container>
            <div className="section-header">Create Contact</div>

            {needsEmailInput &&
                <div>
                    <div>Please enter the email address of the contact you would like to add</div>
                    <label className="input-label">Email</label>
                    <input className="input-text" onChange={(e) => {
                        setValue('email', e.target.value)
                    }}/>
                    {invalidEmail &&
                        <div>The email you entered is not valid, or does not match a known company</div>
                    }
                    <Button onClick={() => validateEmail()}>Next</Button>
                </div>
            }

            {!needsEmailInput && !contact.company && !contact.profile &&
                <div>
                    <div>We did not find a matching company for email domain {emailDomain}, please enter the company
                        name
                    </div>
                    <label htmlFor="txtFirstName" className="input-label">Company Name</label>
                    <input id="txtCompanyName" value={contact.companyName} placeholder="Name of the company to add"
                           onChange={(e) => setValue('companyName', e.target.value)} className="input-text"/>
                    <Button onClick={() => createCompany()}>Next</Button>
                </div>
            }

            {!needsEmailInput && contact.profile &&
                <>
                    <Button onClick={() => editComplete()}>&lt;&lt; Go Back</Button>
                    <p>&nbsp;</p>
                    <div>We found an existing contact that matches the email you entered</div>


                    <ContactDetail contact={contact.profile}/>

                </>
            }

            {!needsEmailInput && contact.company &&
                <>
                    <CompanyTile company={contact.company}/>

                    <div>
                        <label className="input-label">Email</label>
                        <div>{contact.email}</div>
                    </div>
                    <ProfileEdit editComplete={editComplete} companyId={contact.company.id} email={contact.email}/>
                </>
            }
        </Container>
    )
}

