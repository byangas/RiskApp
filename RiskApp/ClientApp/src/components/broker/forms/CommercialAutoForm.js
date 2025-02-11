import {formatNumber} from "../../utils/numberUtils";
import React from "react";

export function CommercialAutoForm({setValue, setNumberValue, insurance}) {

    return (

        <div>
            <div className="section-header">Commercial Auto</div>
            <div>
                <label className="input-label">Total Vehicles </label>
                <input className="input-text"
                       value={formatNumber(insurance.totalVehicles)} onChange={(e) => {
                    setNumberValue('totalVehicles', e.target.value)
                }}/>
            </div>
            <div>
                <label className="input-label">Private Passenger </label>
                <input className="input-text"
                       value={formatNumber(insurance.privatePassenger)} onChange={(e) => {
                    setNumberValue('privatePassenger', e.target.value)
                }}/>
            </div>

            <div>
                <label className="input-label">Light Trucks </label>
                <input className="input-text"
                       value={formatNumber(insurance.lightTrucks)} onChange={(e) => {
                    setNumberValue('lightTrucks', e.target.value)
                }}/>
            </div>
            <div>
                <label className="input-label">Heavy Trucks </label>
                <input className="input-text"
                       value={formatNumber(insurance.heavyTrucks)} onChange={(e) => {
                    setNumberValue('heavyTrucks', e.target.value)
                }}/>
            </div>
            <div>
                <label className="input-label">Travel Radius (Miles)</label>
                <input className="input-text"
                       value={formatNumber(insurance.travelRadius)} onChange={(e) => {
                    setNumberValue('travelRadius', e.target.value)
                }}/>
            </div>
            <div>
                <label className="input-label">Additional Information </label>
                <textarea placeholder="Enter Additional Information or Notes" className="input-text-area" lines="3"
                          value={insurance.additionalInformation} onChange={(e) => {
                    setValue('additionalInformation', e.target.value)
                }}/>
            </div>
        </div>
    )
}