import {formatNumber} from "../../utils/numberUtils";
import React from "react";

export function BuildersRiskInsuranceForm({insurance, setValue, setNumberValue}) {

    return (

        <div>
            <div className="section-header">Builders Risk</div>
            <div>
                <label className="input-label">Building Value ($)</label>
                <input className="input-text" value={formatNumber(insurance.buildingValue)} onChange={(e) => {
                    setNumberValue('buildingValue', e.target.value)
                }}/>
            </div>
            <div>
                <label className="input-label">Contents Value ($)</label>
                <input className="input-text" value={formatNumber(insurance.contentsValue)} onChange={(e) => {
                    setNumberValue('contentsValue', e.target.value)
                }}/>
            </div>

            <div>
                <label className="input-label">Additional Information </label>
                <textarea className="input-text-area" lines="3" value={insurance.additionalInformation}
                          onChange={(e) => {
                              setValue('additionalInformation', e.target.value)
                          }}/>
            </div>
        </div>

    )
}