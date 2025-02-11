import React from "react";
import {formatNumber} from "../utils/numberUtils";

export default function PolicyTile(props) {

    const {policy, clickHandler} = props;

    return (
        <div onClick={() => {
            clickHandler(policy);
        }} className="tile">
            <div className="tileContent">
                <div className='insurance-title'>Policy - {policy.status} </div>
                <div>{policy.description}</div>
                {policy.detail && (
                    <AppetiteFitRequestForm insurance={policy.detail.insurance}/>
                )}
                <div className="insurance-created insurance-label">Created: {policy.createdDate}</div>

            </div>
        </div>
    );
}


export function AppetiteFitRequestForm({insurance}) {

    const generalLiability = insurance.generalLiability;
    const commercialAuto = insurance.commercialAuto;
    const commercialProperty = insurance.commercialProperty;
    const workersComp = insurance.workersComp;
    const buildersRisk = insurance.buildersRisk;
    const umbrellaLiability = insurance.umbrellaLiability;
    const specialty = insurance.specialty;

    return (
        <>

            {generalLiability &&
                <div className="insurance-header">
                    <div>General Liability</div>
                    <div className="insurance-tile-content">
                        <div>
                            <div className="insurance-label">Gross Sales</div>
                            <div>${formatNumber(generalLiability.grossSales)}</div>
                        </div>
                        <div>
                            <div className="insurance-label">Total Payroll</div>
                            <div>${formatNumber(generalLiability.totalPayroll)}</div>
                        </div>
                        <div>
                            <div className="insurance-label">Square Ft</div>
                            <div>{formatNumber(generalLiability.squareFootage)}</div>
                        </div>
                    </div>
                </div>
            }
            {
                commercialAuto &&
                <div className="insurance-header">
                    <div>Commercial Auto</div>
                    <div className="insurance-tile-content">
                        <div>
                            <div className="insurance-label">Total Vehicles</div>
                            <div>{formatNumber(commercialAuto.totalVehicles)}</div>
                        </div>
                        <div>
                            <div className="insurance-label">Travel Radius</div>
                            <div>{formatNumber(commercialAuto.travelRadius)} </div>
                        </div>
                    </div>
                </div>
            }
            {
                commercialProperty &&
                <div className="insurance-header">
                    <div>Commercial Property</div>
                    <div className="insurance-tile-content">
                        {commercialProperty.leased && commercialProperty.leased === 'Yes' &&
                            <>
                                <div>Leased Property</div>
                                <div>
                                    <div className="insurance-label">Personal Property<br/> Replacement Costs
                                    </div>
                                    <div>${formatNumber(commercialProperty.personalPropertyReplacementCosts)}</div>
                                </div>
                            </>}
                        {(!commercialProperty.leased || commercialProperty.leased !== 'Yes') &&
                            <>
                                <div>
                                    <div className="insurance-label">Square Ft</div>
                                    <div>{formatNumber(commercialProperty.squareFootage)}</div>
                                </div>
                                <div>
                                    <div className="insurance-label">Sprinklered</div>
                                    <div>{commercialProperty.sprinklered}</div>
                                </div>
                                <div>
                                    <div className="insurance-label">Bldg Replacement Costs</div>
                                    <div>${formatNumber(commercialProperty.replacementCosts)}</div>
                                </div>
                            </>}

                    </div>

                </div>
            }
            {
                workersComp &&
                <div className="insurance-header">
                    <div>Workers Comp</div>
                    <div className="insurance-tile-content">
                        <div>
                            <div className="insurance-label">Class Code</div>
                            <div> {workersComp.governingClassCode}</div>
                        </div>
                        <div>
                            <div className="insurance-label">Total Payroll</div>
                            <div>${formatNumber(workersComp.payroll)}</div>
                        </div>
                    </div>
                </div>
            }
            {
                buildersRisk &&
                <div className="insurance-header">
                    <div>Builders Risk</div>
                    <div className="insurance-tile-content">
                        <div>
                            <div className="insurance-label">Building Value</div>
                            <div>${formatNumber(buildersRisk.buildingValue)}</div>
                        </div>
                        {buildersRisk.contentsValue && (
                            <div>
                                <div className="insurance-label">Total Payroll</div>
                                <div>${formatNumber(buildersRisk.contentsValue)}</div>
                            </div>
                        )}

                    </div>
                </div>
            }

            {
                umbrellaLiability &&
                <div className="insurance-header">
                    <div>Umbrella Liability</div>
                    <div className="insurance-tile-content">
                        <div>
                            <div className="insurance-label"> Total Amount</div>
                            <div>${formatNumber(umbrellaLiability.totalAmount)}</div>
                        </div>
                    </div>
                </div>
            }

            {
                specialty &&
                <div className="insurance-header">
                    <div>Specialty</div>
                    <div className="insurance-tile-content">
                        <div>
                            <div className="insurance-label">Type</div>
                            <div>{specialty.type}</div>
                        </div>
                    </div>

                </div>
            }
        </>
    )
}
