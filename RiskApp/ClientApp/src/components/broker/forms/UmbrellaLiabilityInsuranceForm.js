import {formatNumber} from "../../utils/numberUtils";
import React from "react";

export function UmbrellaLiabilityInsuranceForm({insurance, setValue, setNumberValue}) {
    return (
        <div>
            <div className="section-header">Umbrella Liability</div>
            <div>
                <label className="input-label">Total Dollar Amount ($)</label>
                <input className="input-text" value={formatNumber(insurance.totalAmount)} onChange={(e) => {
                    setNumberValue('totalAmount', e.target.value)
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