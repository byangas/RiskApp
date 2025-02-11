import React, {useEffect, useRef, useState} from 'react';
import Button from 'react-bootstrap/Button'
import {Container} from 'react-bootstrap'
import {useHistory} from 'react-router-dom'
import CustomerService from "../../services/CustomerService";

export default function CustomerEdit(props) {

    // function to signal to parent object to close editor
    const {closeEditor} = props;
    const [customer, setCustomer] = useState( props.customer);
    const [errorMessages, setErrors] = useState({errors: []});
    const frmRef = useRef();
    const history = useHistory();

    useEffect(() => {

    }, []);


    function validateForm() {
        setErrors({errors: []})
        errorMessages.errors = [];
        validateRequiredString(customer.firmName, "Company Name")
        validateRequiredString(customer.state, "State");
        validateRequiredString(customer.zip, "Zip Code");
        validateRequiredString(customer.industry, "Industry")
        validateRequiredString(customer.industryDetail, "Industry Details")

        return errorMessages.errors.length === 0
    }

    function validateRequiredString(value, fieldName) {
        if (!value || value.length === 0) {
            const {errors} = errorMessages;
            errors.push(fieldName + " is required.")
            setErrors({errors: errors})
            return false;
        }
        return true;
    }


    function save() {
        if (!validateForm()) {
            alert("Please fix errors before continuing");
            return;
        }
        if (customer.id) {
            // todo: make the update work!
            CustomerService.update(customer).then(()=>{
                closeEditor();
            })

        } else {
            CustomerService.create(customer).then(() => {
                alert("Saved successfully")
                //stay on the same view, but add the ID to context. This will force a rerender
                closeEditor();
            }).catch(error => {
                alert("There was an error saving")
                console.log(error)
            });
        }
    }


    function setValue(fieldName, val) {
        const newBiz = {...customer};
        newBiz[fieldName] = val;
        setCustomer(newBiz);
    }

    const errors = errorMessages.errors.map((error, index) => {
        return <div className="errors" key={index}>{error}</div>
    })

    function deleteAccount() {
        CustomerService.delete(customer.id).then(()=> {
            alert("Account and related policies are deleted.")
            history.goBack();
        })
    }

    return (
        <Container>
            <form name='frmMain' ref={frmRef}>
                <div className="form-errors">
                    {errors}
                </div>
                <div>
                    <label className="input-label">Company Name</label>
                    <input type="text" className="input-text" value={customer.firmName} onChange={(e) => {
                        setValue('firmName', e.target.value)
                    }}/>
                </div>
                <div>
                    <label className="input-label">Primary Contact</label>
                    <input type="text" className="input-text" value={customer.primaryContact} onChange={(e) => {
                        setValue('primaryContact', e.target.value)
                    }}/>
                </div>
                <div>
                    <label className="input-label">Primary Contact Phone</label>
                    <input type="text" className="input-text" value={customer.primaryContactPhone} onChange={(e) => {
                        setValue('primaryContactPhone', e.target.value)
                    }}/>
                </div>
                <div>
                    <label className="input-label">Primary Contact Email</label>
                    <input type="text" className="input-text" value={customer.email} onChange={(e) => {
                        setValue('email', e.target.value)
                    }}/>
                </div>
                <div>
                    <label className="input-label">Address </label>
                    <input className="input-text" value={customer.address} onChange={(e) => {
                        setValue('address', e.target.value)
                    }}/>
                </div>
                <div>
                    <label className="input-label">City</label>
                    <input className="input-text" value={customer.city} onChange={(e) => {
                        setValue('city', e.target.value)
                    }}/>
                </div>

                <div>
                    <label className="input-label">State</label>
                    <input className="input-text" value={customer.state} onChange={(e) => {
                        setValue('state', e.target.value)
                    }}/>
                </div>
                <div>
                    <label className="input-label">Zip Code </label>
                    <input className="input-text" value={customer.zip} onChange={(e) => {
                        setValue('zip', e.target.value)
                    }}/>
                </div>
                <div>
                    <label className="input-label">Industry </label>
                    <select className="input-text" name="industry" value={customer.industry}
                            onChange={(e) => setValue('industry', e.target.value)}>
                        <option value='' readOnly={true} style={{color: "blue!important"}} hidden={true}>Please select
                            an industry
                        </option>

                        <option>Accommodation and Food Services</option>
                        <option>Administrative, Support, Waste Management and Remediation Services</option>
                        <option>Agriculture, Forestry, Fishing and Hunting</option>
                        <option>Arts, Entertainment, and Recreation</option>
                        <option>Automotive</option>
                        <option>Construction</option>
                        <option>Educational Services</option>
                        <option>Finance and Insurance</option>
                        <option>Health Care and Social Assistance</option>
                        <option>Information</option>
                        <option>Management of Companies and Enterprises</option>
                        <option>Manufacturing</option>
                        <option>Mining, Quarrying, and Oil and Gas Extraction</option>
                        <option>Professional, Scientific, and Technical Services</option>
                        <option>Public Administration</option>
                        <option>Real Estate and Rental and Leasing</option>
                        <option>Retail Trade</option>
                        <option>Transportation and Warehousing</option>
                        <option>Utilities</option>
                        <option>Wholesale Trade</option>
                        <option>Other Services</option>

                    </select>
                </div>
                <div>
                    <label className="input-label">Industry Details</label>
                    <div className="tips">Add information about the industry that would help the Carrier understand the
                        industry. Example: If you select Agriculture, put "Farming" or "Packing" in this field
                    </div>
                    <input placeholder="More information about the Industry" className="input-text"
                           value={customer.industryDetail} onChange={(e) => {
                        setValue('industryDetail', e.target.value)
                    }}/>
                </div>
                <div>
                    <label className="input-label">Additional Information </label>
                    <textarea placeholder="Enter Additional Information or Notes" className="input-text-area" lines="3"
                              value={customer.additionalInformation} onChange={(e) => {
                        setValue('additionalInformation', e.target.value)
                    }}/>
                </div>


                <div className="form-errors">
                    {errors}
                </div>

                <div className="button-bar">
                    <Button variant="primary" onClick={() => {
                        save()
                    }}>Save</Button>&nbsp;
                    {customer && customer.id &&
                    <Button variant="danger" onClick={() => {
                        deleteAccount()
                    }}>Delete</Button>
                    }
                    &nbsp;<Button variant="secondary" onClick={() => {
                        closeEditor();
                    }}>Cancel</Button>

                </div>

            </form>
        </Container>
    );

}



