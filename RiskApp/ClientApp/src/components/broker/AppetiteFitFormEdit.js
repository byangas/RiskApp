import React, {useEffect, useState} from 'react';
import Button from 'react-bootstrap/Button'
import {numberFromString} from '../utils/numberUtils'
import {SpecialtyInsuranceForm} from "./forms/SpecialtyInsuranceForm";
import {GeneralLiabilityForm} from "./forms/GeneralLiabilityForm";
import {CommercialPropertyForm} from "./forms/CommercialPropertyForm";
import {WorkersCompensationForm} from "./forms/WorkersCompensationForm";
import {BuildersRiskInsuranceForm} from "./forms/BuildersRiskInsuranceForm";
import {UmbrellaLiabilityInsuranceForm} from "./forms/UmbrellaLiabilityInsuranceForm";
import {CommercialAutoForm} from "./forms/CommercialAutoForm";
import PolicyService from "../../services/PolicyService";
import {useHistory, useParams} from "react-router-dom";
import {Container} from "react-bootstrap";


export default function AppetiteFitFormEdit() {
    const {policyId} = useParams()
    const [policy, setPolicy] = useState({});
    const [errorMessages, setErrors] = useState({errors: []});

    const history = useHistory()


    useEffect(() => {

        PolicyService.getPolicy(policyId).then(policy => {
            if (policy.detail) {
                setPolicy(policy.detail)
            }
        });
    }, []);


    function validateForm() {

        setErrors({errors: []})
        errorMessages.errors = [];

        if (!policy.insurance) {
            setErrors({errors: ["Please Select at least one line of Insurance"]});
            return false;
        }
        let sectionTitle = ""
        if (policy.insurance.workersComp) {
            sectionTitle = "Workers Comp: "
            const insurance = policy.insurance.workersComp;
            validateRequiredString(insurance.governingClassCode, sectionTitle + "Governing Class Code")
            validateRequiredString(insurance.mod, sectionTitle + "Mod")
            validateRequiredNumber(insurance.payroll, sectionTitle + "Payroll");
        }
        if (policy.insurance.commercialAuto) {
            sectionTitle = "Commercial Auto: "
            const insurance = policy.insurance.commercialAuto;
            validateRequiredString(insurance.travelRadius, sectionTitle + "Travel Radius")
            validateRequiredNumber(insurance.totalVehicles, sectionTitle + "Total Vehicles");
        }
        if (policy.insurance.commercialProperty) {
            sectionTitle = "Commercial Property: "
            const insurance = policy.insurance.commercialProperty;
            if (insurance.leased && insurance.leased === 'Yes') {
                validateRequiredNumber(insurance.personalPropertyReplacementCosts, sectionTitle + "Personal Property Replacement Costs");
            } else {
                validateRequiredNumber(insurance.squareFootage, sectionTitle + "Square Footage")
                validateRequiredString(insurance.constructionType, sectionTitle + "Construction Type");
                validateRequiredString(insurance.sprinklered, sectionTitle + "Sprinklered");
                validateRequiredString(insurance.roofType, sectionTitle + "Roof Type");
                validateRequiredNumber(insurance.replacementCosts, sectionTitle + "Building Replacement Costs");
            }
        }
        if (policy.insurance.buildersRisk) {
            sectionTitle = "Builders Risk: "
            const insurance = policy.insurance.buildersRisk;
            validateRequiredNumber(insurance.buildingValue, sectionTitle + "Building Value")
        }
        if (policy.insurance.umbrellaLiability) {
            sectionTitle = "Umbrella Liability: "
            const insurance = policy.insurance.umbrellaLiability;
            validateRequiredNumber(insurance.totalAmount, sectionTitle + "Total Dollar Amount")
        }
        if (policy.insurance.generalLiability) {
            sectionTitle = "General Liability: "
            const insurance = policy.insurance.generalLiability;
            validateRequiredNumber(insurance.grossSales, sectionTitle + "Gross Sales")
        }
        if (policy.insurance.specialty) {
            //require additionalInfo
            sectionTitle = "Specialty: "
            const insurance = policy.insurance.specialty;
            validateRequiredString(insurance.type, sectionTitle + "Type");

        }

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

    function validateRequiredNumber(value, fieldName) {
        if (!value || isNaN(value) || value === 0) {
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
        PolicyService.updateAppetite(policyId, policy).then(() => {
            history.goBack();
        }).catch((error) => {
            console.log(error)
            alert("Something went horribly wrong.")
        })

    }

    function setValue(fieldName, val) {
        const newBiz = {...policy};
        newBiz[fieldName] = val;
        setPolicy(newBiz);
    }

    // function takes a string value to set, and converts
    // it into a number value so that when we set the value
    // in the JS object, it is a numeric value not a string value
    // we need to have a number value for filtering criteria and reporting
    function setSubFormValueAsNumber(subform, fieldName, val) {
        setSubFormValue(subform, fieldName, numberFromString(val));
    }

    function setSubFormValue(subform, fieldName, val) {
        //make sure that there is an insurance object to add value to
        if (!policy.insurance) {
            policy.insurance = {};
        }
        let insurance = policy.insurance[subform]
        //ensure a subForm exists
        if (!insurance)
            insurance = {}
        insurance[fieldName] = val
        setPolicy({...policy})
    }

    //higher order function that essentially prefills the subform
    function setValueFor(subForm) {
        return function (fieldName, val) {
            setSubFormValue(subForm, fieldName, val);
        }
    }

    function setNumberValueFor(subForm) {
        return function (fieldName, val) {
            setSubFormValueAsNumber(subForm, fieldName, val);
        }
    }

    function toggleSection(fieldName, target) {
        let newValue;

        if (target.checked) {
            newValue = {};
        }
        const newPolicy = {...policy};
        if (!newPolicy.insurance) {
            newPolicy.insurance = {};
        }
        newPolicy.insurance[fieldName] = newValue;
        setPolicy(newPolicy);

    }

    const errors = errorMessages.errors.map((error, index) => {
        return <div className="errors" key={index}>{error}</div>
    })


    function deleteAppetite() {
        PolicyService.deleteAppetite(policyId).then(()=> {
            history.goBack()
        })
    }

    return (
        <Container>
            <div>Please fill out the information about the policy that you want to send for appetite fit.</div>

            <form name='frmMain'>
                <div className="section-header">Policy</div>
                <div className="form-errors">
                    {errors}
                </div>
                <div>
                    <label className="input-label">Policy Notes </label>
                    <textarea placeholder="Enter Additional Information or Notes for the underwriter about the policy" className="input-text-area" lines="3"
                              value={policy.additionalInformation} onChange={(e) => {
                        setValue('additionalInformation', e.target.value)
                    }}/>
                </div>


                {policy.insurance && policy.insurance.workersComp && (
                    <WorkersCompensationForm insurance={policy.insurance.workersComp}
                                             setValue={setValueFor('workersComp')}
                                             setNumberValue={setNumberValueFor('workersComp')}
                    />)
                }

                {policy.insurance && policy.insurance.commercialAuto && (
                    <CommercialAutoForm insurance={policy.insurance.commercialAuto}
                                        setValue={setValueFor('commercialAuto')}
                                        setNumberValue={setNumberValueFor('commercialAuto')}
                    />)
                }


                {policy.insurance && policy.insurance.commercialProperty && (
                    <CommercialPropertyForm insurance={policy.insurance.commercialProperty}
                                            setValue={setValueFor('commercialProperty')}
                                            setNumberValue={setNumberValueFor('commercialProperty')}
                    />)
                }

                {policy.insurance && policy.insurance.generalLiability && (
                    <GeneralLiabilityForm insurance={policy.insurance.generalLiability}
                                          setValue={setValueFor('generalLiability')}
                                          setNumberValue={setNumberValueFor('generalLiability')}

                    />)
                }


                {policy.insurance && policy.insurance.buildersRisk && (
                    <BuildersRiskInsuranceForm insurance={policy.insurance.buildersRisk}
                                               setValue={setValueFor('buildersRisk')}
                                               setNumberValue={setNumberValueFor('buildersRisk')}/>)}

                {policy.insurance && policy.insurance.umbrellaLiability && (
                    <UmbrellaLiabilityInsuranceForm insurance={policy.insurance.umbrellaLiability}
                                                    setValue={setValueFor('umbrellaLiability')}
                                                    setNumberValue={setNumberValueFor('umbrellaLiability')}

                    />)
                }

                {policy.insurance && policy.insurance.specialty && (
                    <SpecialtyInsuranceForm insurance={policy.insurance.specialty}
                                            setValue={setValueFor('specialty')}/>)}

                <div className="checkbox_holder">

                    <div className="checkbox">
                        <input type="checkbox" id="workerComp"
                               checked={!!(policy.insurance !== undefined && policy.insurance.workersComp)}
                               onChange={(e) => {
                                   toggleSection('workersComp', e.target)
                               }}/>
                        <label htmlFor='workerComp'>Add Workers Comp</label>
                    </div>
                    <div className="checkbox">
                        <input type="checkbox" id="commercialAuto"
                               checked={!!(policy.insurance !== undefined && policy.insurance.commercialAuto)}
                               onChange={(e) => {
                                   toggleSection('commercialAuto', e.target)
                               }}/>
                        <label htmlFor='commercialAuto'>Add Commercial Auto</label>
                    </div>
                    <div className="checkbox">
                        <input type="checkbox" id="commercialProperty"
                               checked={!!(policy.insurance !== undefined && policy.insurance.commercialProperty)}
                               onChange={(e) => {
                                   toggleSection('commercialProperty', e.target)
                               }}/>
                        <label htmlFor='commercialProperty'>Add Commercial Property</label>
                    </div>
                    <div className="checkbox">

                        <input type="checkbox" id="generalLiability"
                               checked={!!(policy.insurance !== undefined && policy.insurance.generalLiability)}
                               onChange={(e) => {
                                   toggleSection('generalLiability', e.target)
                               }}/>
                        <label htmlFor='generalLiability'>Add General Liability</label>
                    </div>
                    <div className="checkbox">
                        <input type="checkbox" id="buildersRisk"
                               checked={!!(policy.insurance !== undefined && policy.insurance.buildersRisk)}
                               onChange={(e) => {
                                   toggleSection('buildersRisk', e.target)
                               }}/>
                        <label htmlFor='buildersRisk'>Add Builders Risk Insurance</label>
                    </div>
                    <div className="checkbox">
                        <input type="checkbox" id="umbrella"
                               checked={!!(policy.insurance !== undefined && policy.insurance.umbrellaLiability)}
                               onChange={(e) => {
                                   toggleSection('umbrellaLiability', e.target)
                               }}/>
                        <label htmlFor='umbrella'>Add Umbrella Liability</label>
                    </div>
                    <div className="checkbox">
                        <input type="checkbox" id="specialty"
                               checked={!!(policy.insurance !== undefined && policy.insurance.specialty)}
                               onChange={(e) => {
                                   toggleSection('specialty', e.target)
                               }}/>
                        <label htmlFor='specialty'>Add Specialty Insurance</label>
                    </div>
                </div>

                <div className="form-errors">
                    {errors}
                </div>

                <div className="button-bar">
                    <Button variant="primary" onClick={() => {
                        save()
                    }}>Save</Button>

                    <Button variant="danger" onClick={() => {
                        deleteAppetite()
                    }}>Delete</Button>

                    <Button variant="secondary" onClick={() => {
                        history.goBack()
                    }}>Cancel</Button>
                </div>
            </form>
        </Container>
    );
}

