import {formatNumber} from "../../utils/numberUtils";
import React from "react";

export function GeneralLiabilityForm({insurance, setValue, setNumberValue}) {
    return (
        <div>
            <div className="section-header">General Liability</div>
            <div>
                <label className="input-label">Gross Sales ($)</label>
                <input className="input-text"
                       value={formatNumber(insurance.grossSales)} onChange={(e) => {
                    setNumberValue('grossSales', e.target.value)
                }}/>
            </div>
            <div>
                <label className="input-label">Total Payroll ($)</label>
                <input className="input-text"
                       value={formatNumber(insurance.totalPayroll)} onChange={(e) => {
                    setNumberValue('totalPayroll', e.target.value)
                }}/>
            </div>
            <div>
                <label className="input-label">Square Footage</label>
                <input className="input-text"
                       value={formatNumber(insurance.squareFootage)} onChange={(e) => {
                    setNumberValue('squareFootage', e.target.value)
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