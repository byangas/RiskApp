import React, {useContext, useEffect, useRef, useState} from "react";
import {Button, Container, ToggleButton} from "react-bootstrap";
import {useHistory, useParams} from "react-router-dom";
import ContactService from "../../services/ContactService";
import UserContext from "../../UserContext";
import debounce from "../utils/debounce";
import PolicyService from "../../services/PolicyService";
import CustomerHeader from "./CustomerHeader";

const searchContactsDebounced = debounce((myCompanyOnly, searchText, callback) => {
    ContactService.getContacts(myCompanyOnly, searchText).then(contacts => {
        callback(contacts)
    })
})

export default function MarketingSheetAddContact() {
    const [showMyCompanyOnly, setShowMyCompanyOnly] = useState(true);
    const [contactsToAdd, setContactsToAdd] = useState([])
    const [contactList, setContactList] = useState([])
    const {user} = useContext(UserContext)
    const history = useHistory();
    const txtSearchRef = useRef();
    const {policyId, customerId} = useParams();

    useEffect(() => {
        GetContacts();
    }, [showMyCompanyOnly])

    function searchContacts() {
        searchContactsDebounced(showMyCompanyOnly, txtSearchRef.current.value, setContactList)
    }

    function GetContacts() {
        ContactService.getContacts(showMyCompanyOnly, txtSearchRef.current.value).then(data => {
            console.log(data)
            setContactList(data)
        });
    }

    function RemoveContact(contact) {
        const tempArray = [...contactsToAdd];
        const indexOfContact = tempArray.indexOf(contact)
        if (indexOfContact !== -1) {
            tempArray.splice(indexOfContact, 1)
        }
        setContactsToAdd(tempArray)
    }

    function AddToMarketingSheet(contact) {
        const tempArray = [...contactsToAdd];
        const indexOfContact = tempArray.indexOf(contact)
        if (indexOfContact === -1) {
            tempArray.push(contact)
        }
        setContactsToAdd(tempArray)
    }


    const addedContactStyle = {
        backgroundColor: "grey",
        margin: "3px",
        padding: "6px",
        borderRadius: "9px",
        whiteSpace: "nowrap"
    }
    const deleteButtonStyle = {
        backgroundColor: "black",
        padding: "6px",
        margin: "3px",
        borderRadius: "12px",
        height: "12px",
        width: "18px"
    }

    const contactsToAddRender = contactsToAdd.map((contact) => {
        return (
            <span className="clickable" onClick={() => RemoveContact(contact)}
                  style={addedContactStyle}>{contact.company.name} - {contact.name} <span
                style={deleteButtonStyle}>x</span></span>
        )
    })


    const contactListRender = contactList.map((contact) => {
        return (<div onClick={() => AddToMarketingSheet(contact)} className="marketing-contact-add clickable">
            <span>{contact.company.name} - {contact.name}<br/>
                {contact.title} <br/>
                {contact.specialty}
        </span> <img src={`/assets/companylogo/${contact.company.logo}`} style={{width: "100px"}}/></div>
        )})


    function addContacts() {
        let numberOfContactsToAdd = contactsToAdd.length;
        contactsToAdd.forEach((contact) => {
            PolicyService.addMarketingContact(policyId, contact.id).then(result => {
                // decrement counter so that when all the contacts have been processed,
                // we leave this page/screen
                numberOfContactsToAdd = numberOfContactsToAdd - 1;
                if (numberOfContactsToAdd === 0) {
                    history.goBack();
                }
            })
        })
    }

    return (
        <Container>

            <CustomerHeader customerId={customerId}/>
            <div>
                <p>&nbsp;</p>
                <div style={{paddingBottom:"6px"}}>
                    {contactsToAdd.length !== 0 && (
                        <>
                            <Button onClick={addContacts}>Add contacts to Marketing Sheet</Button>&nbsp;
                        </>
                    )}
                    <Button variant="secondary" onClick={() => history.goBack()}>Cancel</Button>
                </div>


                <div style={{padding: "6px "}} className="tile-container border">
                    {contactsToAdd.length === 0 && (
                        <div>Select contacts below to add to Marketing Sheet</div>
                    )}
                    {contactsToAddRender}
                </div>
            </div>

            <p>&nbsp;</p>
            <div>
                Filter:&nbsp;
                <span style={{margin: "0px 12px"}}>
                   <label htmlFor="rdoCompany">{user.company.name} Contacts &nbsp;</label>
                    <input checked type="radio"
                           id='rdoCompany'
                           name="filterBy"
                           value="company" onClick={() => setShowMyCompanyOnly(true)}/>
                </span>
                <span>
                 <label htmlFor="rdoAll">All Contacts &nbsp;</label>
                    <input type="radio" id='rdoAll'
                           name="filterBy"
                           value="all"
                           onClick={() => setShowMyCompanyOnly(false)}/>
                </span>
                <div>
                    <label className="input-label" htmlFor="txtPolicyDescription">Search by Name, Title, or
                        Specialty</label>
                    <input ref={txtSearchRef} onChange={() => searchContacts()} className="input-text" type="text"/>
                </div>
                <div style={{height: "100%"}}>
                    List of contacts to add
                    <div style={{height: "100%"}}>
                        {contactListRender}
                    </div>

                </div>
            </div>
        </Container>
    )
}