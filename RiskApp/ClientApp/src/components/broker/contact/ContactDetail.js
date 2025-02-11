import React, {useContext} from "react";
import {useHistory} from "react-router-dom";
import ContactService from "../../../services/ContactService";
import Button from "react-bootstrap/Button";
import ContactTile from "./ContactTile";
import UserContext from "../../../UserContext";
import ProfileEdit from "../../ProfileEdit";

export default function ContactDetail({contact}) {

    const history = useHistory();
    const {user} = useContext(UserContext);
    function addCompanyContact() {
        ContactService.addCompanyContact(contact.id).then(() => {
            history.goBack()
        })
    }
    function removeCompanyContact() {
        ContactService.removeCompanyContact(contact.id).then(() => {
            history.goBack();
        })
    }



    return (
        <>
            <div>
                {!contact.companyContact &&
                    <Button onClick={() => addCompanyContact()}>Add to {user.company.name} contacts
                        &nbsp;<img style={{height: "35px"}} src="/assets/icons/addCompanyContact.svg"/></Button>
                }
                {contact.companyContact &&
                    <Button onClick={() => removeCompanyContact()}>Remove
                        from {user.company.name} contacts</Button>
                }
            </div>
            <p>&nbsp;</p>
                <div>
                    <label className="input-label">Email</label>
                    <div  >
                        {contact.email}
                    </div>
                </div>

            <ProfileEdit profile={contact} editComplete={()=> history.goBack()} />
        </>
    )
}