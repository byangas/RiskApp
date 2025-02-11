import {useHistory} from "react-router-dom";
import React, {useEffect, useState} from "react";
import Spinner from  "../controls/Spinner"
import CustomerService from "../../services/CustomerService";


export default function CustomerHeader({customerId}) {

    const history = useHistory();
    const [customer, setCustomer] = useState()

    useEffect(()=> {
        CustomerService.getCustomer(customerId).then(c=> setCustomer(c)).catch((err)=> {
            console.log(err)
        })
    },[])

    function goToDetail() {
        history.push(`/broker/customer/${customerId}`)
    }

    if(!customer) {
        return <Spinner/>
    }

    return (
        <div onClick={goToDetail} >
            <label className="text-info">Customer:&nbsp;</label>
             {customer.firmName} &nbsp;
        </div>
    )
}