import React from "react";
import {Container} from "react-bootstrap";

export default function Supplementals() {
    const documents = ["Professional Liability",
        "ERISA",
        "Workers Compensation",
        "Winery",
        "Garage Keeper’s",
        "Contractors",
        "Academic Schools",
        "Religious Organizations",
        "Non-Profit",
        "Products Liability",
        "Residential Care",
        "Habitational"
    ]
 
    return (
        <Container>
            <div className="section-header">Supplemental</div>
            <p>
            </p>
            <div className="tile-container">

                { documents.map((val, index) => {
                    return (
                    <div key={index} style={{backgroundColor: "#363f55", borderRadius: "6px", width: "200px", marginBottom:"12px", marginRight:"12px"}}>
                    <div style={{
                    backgroundColor: "darkslategray",
                    padding: "3px 6px",
                    borderRadius: "3px 3px 0px 0px"
                }}>
                {val}
                    </div>
                    <div style={{textAlign: "center"}}>
                    <img style={{width: "75px", padding: "6px"}} src="/assets/icons/docs.svg"/>
                    </div>
                    </div>
                    )
                })
                }

            </div>
        </Container>
    )
}