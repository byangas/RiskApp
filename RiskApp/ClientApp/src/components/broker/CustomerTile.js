import {useHistory} from "react-router-dom";
import React from "react";


export default function CustomerTile({customer}) {

    const history = useHistory();

    function goToDetail() {
        history.push(`/broker/customer/${customer.id}`)
    }

    return (
        <div onClick={goToDetail} className="tile">

            <div className="tileContent">
                <div className="statusbar">
                    {customer.firmName} &nbsp;
                </div>
                <div>{customer.primaryContact}</div>
                <div>{customer.primaryContactPhone}</div>
                <div>{customer.email}</div>
                <div>
                    {customer.address} <br/>{customer.city}, {customer.state} {customer.zip}
                </div>
            </div>
        </div>
    )
}