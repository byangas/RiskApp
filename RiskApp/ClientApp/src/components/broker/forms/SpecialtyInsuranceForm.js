import React from "react";

export function SpecialtyInsuranceForm({insurance, setValue}) {
    return (
        <>
            <div className="section-header">Specialty</div>
            <div>
                <label className="input-label">Type</label>
                <select className="input-text"
                        value={insurance.type} onChange={(e) => {
                    setValue('type', e.target.value)
                }}>
                    <option value=''>Please select a type</option>
                    <option>Bonds</option>
                    <option>Cyber</option>
                    <option>Directors and Officers</option>
                    <option>Employers Practices Liability</option>
                    <option>Errors and Omissions</option>
                    <option>Flood</option>
                    <option>Inland Marine</option>
                    <option>Other</option>

                </select>
            </div>
            <div>
                <label className="input-label">Additional Information </label>
                <textarea placeholder="Enter Additional Information or Notes" className="input-text-area" lines="3"
                          value={insurance.additionalInformation} onChange={(e) => {
                    setValue('additionalInformation', e.target.value)
                }}/>
            </div>

        </>
    )
}