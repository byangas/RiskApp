import React from "react";
import {Link} from "react-router-dom";

export default function CompanyTile(props) {
    const {company} = props;
    return (
        <Link to={`/broker/contacts/company/${company.id}`}>
            <div className="tile" style={{textAlign: "left", minHeight: "150px"}}>
                <div className="tileContent">
                    <div className="tile-title">{company.name}</div>
                    <div style={{textAlign: "center"}}>
                        <img style={{margin: "10px", width: "120px"}} src={`/assets/companylogo/${company.logo}`}/>
                    </div>
                    <div> </div>
                </div>
            </div>
        </Link>
    )
}