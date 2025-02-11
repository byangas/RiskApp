import {formatNumber} from "../../utils/numberUtils";
import React from "react";

export function WorkersCompensationForm({insurance, setValue, setNumberValue}) {

    return (

        <div>
            <div className="section-header">Workers Comp</div>

            <div>
                <label className="input-label">Governing Class Code</label>
                <input className="input-text"
                       value={insurance.governingClassCode} onChange={(e) => {
                    setValue('governingClassCode', e.target.value)
                }}/>
            </div>
            <div>
                <label className="input-label">Payroll ($)</label>
                <input className="input-text" value={formatNumber(insurance.payroll)} onChange={(e) => {
                    setNumberValue('payroll', e.target.value)
                }}/>
            </div>
            <div>
                <label className="input-label">Mod </label>
                <input className="input-text" value={insurance.mod} onChange={(e) => {
                    setValue('mod', e.target.value)
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