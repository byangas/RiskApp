import React, {useEffect, useState} from "react";
import {Button, Container} from "react-bootstrap";
import CustomerService from "../../services/CustomerService";
import CustomerTile from "./CustomerTile";
import {useHistory} from "react-router-dom";

export default function Customers() {

    const [customers, setCustomers] = useState()
     const history = useHistory()

    useEffect(() => {
        CustomerService.getCustomers().then(cstmrs => {
            const customerTiles = cstmrs.map(customer => {
                return <CustomerTile key={customer.id} customer={customer}/>
            })
            setCustomers(customerTiles)
        })

    }, [])

    return (
        <Container>
            <div className="section-header">Customers</div>
            <Button onClick={()=> history.push('/broker/customer/new')}>Create new Customer</Button>
            <div>
                {customers}
            </div>
        </Container>

    )
}