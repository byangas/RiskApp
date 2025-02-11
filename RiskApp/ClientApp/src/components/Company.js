import React, {useContext, useEffect, useState} from "react";
import {useParams} from "react-router-dom";
import CompanyService from "../services/CompanyService";
import Spinner from '../components/controls/Spinner'
import {Button, Container} from "react-bootstrap";
import ContactService from "../services/ContactService";
import BrokerContacts from "./broker/contact/BrokerContacts";
import CarrierAppointmentService from "../services/CarrierAppointmentService";
import UserContext from "../UserContext";

export default function Company() {

    const {companyId} = useParams();
    const [company, setCompany] = useState();

    useEffect(()=> {
        CompanyService.getCompanyById(companyId).then(cmpy => {
            setCompany(cmpy)
        })
    },[])

    if(!company) {
        return <Spinner/>
    }
    return (
        <Container>
            <h2>{company.name}</h2>
            <CarrierAppointment carrierId={companyId}/>
            <BrokerContacts companyId={companyId} />

        </Container>

    )
}

function CarrierAppointment({carrierId}) {

    function LoadCarrierAppointment() {
        setLoading(true);
        CarrierAppointmentService.getCarrierAppointment(carrierId).then(appointment => {
            setLoading(false);
            setCarrierAppointment(appointment);
        })
    }

    function AddCarrierAppointment() {
        setLoading(true)
        CarrierAppointmentService.addCarrierAppointment(carrierId).then((appointment)=> {
            alert("Added");
            setLoading(false)
            setCarrierAppointment(appointment)
        })

    }
    const [loading, setLoading] = useState(true);
    const [carrierAppointment, setCarrierAppointment] = useState()
    const {user} = useContext(UserContext)

    useEffect(()=> {
        LoadCarrierAppointment();
    },[])

    if(loading) {
        return (<Spinner/>)
    }

    if(!carrierAppointment) {
        return(
            <>
                <div>No appointment</div>
                <Button onClick={AddCarrierAppointment}>Add Appointed Carrier to {user.company.name}</Button>
                <p></p>
            </>
        )
    }


    /**
     * @param {string} code
     */
    function UpdateBrokerageNotes(brokerNotes) {
        const temp = {...carrierAppointment};
        temp.brokerNotes = brokerNotes;
        setCarrierAppointment(temp);
    }

    function UpdateAppointment() {
        CarrierAppointmentService.updateCarrierAppointment(carrierId, carrierAppointment.brokerNotes).then(()=>
        {
            alert("updated")
        })
    }

    function DeleteAppointment() {
        CarrierAppointmentService.deleteAppointment(carrierId).then(()=> {
            setCarrierAppointment(undefined);
        })
    }

    const createDate = new Date(carrierAppointment.createdDate);




    return (
        <>
            <label className="input-label">Broker Notes</label>
            <textarea rows="3" className="input-text" onChange={(e)=> { UpdateBrokerageNotes(e.target.value)}} value={carrierAppointment.brokerNotes}/>
<div>Notes are viewable to anyone in brokerage, but not visible to anyone outside the brokerage </div>
            <div>Created By  {carrierAppointment.createdByName}</div>
            <div>Created Date {createDate.toLocaleDateString("medium")}</div>
            <Button onClick={()=> UpdateAppointment()}>Update</Button> <Button onClick={()=> DeleteAppointment()} variant="danger">Delete (remove appointment)</Button>
        </>
    )
}