import React, {useContext, useEffect, useState} from 'react'
import {useHistory} from 'react-router-dom';
import ContactService from "../../../services/ContactService";
import UserContext from "../../../UserContext";
import ContactTile from './ContactTile';

export default function BrokerContacts({companyId}) {

    const [items, setItems] = useState([]);
    const [sortBy, setSortBy] = useState(!companyId ? "company_asc" : "name_asc")
    const {user} = useContext(UserContext)
    const [showMyCompanyOnly, setShowMyCompanyOnly] = useState(true);
    const history = useHistory()

    function setFilterBy(filterBy) {
        setShowMyCompanyOnly(filterBy !== "all")
    }

    function ShowContactDetails(contact) {
        history.push('/broker/contacts/' + contact.id)
    }

    function GetContacts() {
        ContactService.getContacts(showMyCompanyOnly, "", sortBy, companyId).then(data => {
            const items = data.map((contact, i) => {
                return <div onClick={() => ShowContactDetails(contact)}>
                    <ContactTile key={i} contact={contact}></ContactTile>
                </div>
            });
            setItems(items);
        });
    }

    useEffect(() => {
        GetContacts();
    }, [showMyCompanyOnly, sortBy])

    return (
        <>

            <div style={{display: "flex", justifyContent: "left"}}>
                <span>
                    <label htmlFor="rdoCompany">{user.company.name} Contacts </label>&nbsp;
                    <input type="radio"
                           defaultChecked={showMyCompanyOnly}
                           id='rdoCompany'
                           name="filterBy"
                           value="company"
                           onChange={(e) => setFilterBy(e.target.value)}/>
                </span>
                <span>
                    &nbsp;<label htmlFor="rdoAll">All Contacts </label>&nbsp;
                    <input type="radio" id='rdoAll'
                           defaultChecked={!showMyCompanyOnly}
                           name="filterBy"
                           value="all"
                           onChange={(e) => setFilterBy(e.target.value)}/>
                </span>
            </div>
            <div> Sort By:
                <select value={sortBy} onChange={(e) => setSortBy(e.target.value)}>
                    {!companyId && (
                        <>
                            <option value="company_asc">Company (A-Z)</option>
                            <option value="company_desc">Company (Z-A)</option>
                        </>
                    )}
                    <option value="name_asc">Last Name (A-Z)</option>
                    <option value="name_desc">Last Name (Z-A)</option>
                </select>
            </div>
            <div className="tile-container">
                {items}
            </div>
        </>
    )
}

