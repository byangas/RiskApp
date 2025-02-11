import React from 'react';
import { Button } from "react-bootstrap";
export default function ContactTile(props) {
    const { contact } = props;

    const iconUrl = contact.companyContact ? "companyContact.svg" : "addCompanyContact.svg";

    return (

            <div className="tile">
                <div className="tileContent" style={{ textAlign: "left" }}>
                    <div className="tile-title ">
                        <span>{contact.name}</span>
                            <img style={{ height: "28px", width: "28px" }} src={"/assets/icons/" + iconUrl} />
                    </div>
                    <div style={{ display: "flex", flexDirection: "row" }}>
                        <div className="tileContent">
                            <div>{contact.title}</div>
                            <div>{contact.company.name}</div>
                            {  contact.specialty &&
                                <div>Specialties: {contact.specialty}</div>
                            }
                            <div>Phone <a href={`tel:${contact.phone}`}>{contact.phone}</a></div>
                            { contact.mobilePhone && (
                                <div>Mobile <a href={`tel:${contact.mobilePhone}`}>{contact.mobilePhone}</a></div>
                            )}
                            <div>Email <a href={`mailto:${contact.email}`}>{contact.email}</a></div>
                        </div>
                        <a target="_blank" href={contact.company.website}><img style={{ margin: "3px", maxWidth: "100px" }}
                            src={"/assets/companylogo/" + contact.company.logo}
                            height="45" />
                        </a>
                    </div>

                </div>
            </div>

    );
}
