import {formatNumber} from "../../utils/numberUtils";
import React from "react";

export function CommercialPropertyForm({setValue, setNumberValue, insurance}) {

    return (
        <div>
            <div className="section-header">Commercial Property</div>
            <div>
                <label className="input-label" htmlFor="leasedBuilding">Building Is Leased</label>
                <input name="leasedBuilding" id="leasedBuilding" type="checkbox"
                       checked={insurance.leased === 'Yes'} onChange={(e) => {
                    setValue('leased', e.target.checked ? 'Yes' : "No")
                    if (!e.target.checked) {
                        setNumberValue('personalPropertyReplacementCosts', '')
                    }
                }}/> Yes
            </div>
            {insurance.leased && insurance.leased === 'Yes' &&
            <div>
                <label className="input-label">Personal Property Replacement Costs ($)</label>
                <input className="input-text"
                       value={formatNumber(insurance.personalPropertyReplacementCosts)} onChange={(e) => {
                    setNumberValue('personalPropertyReplacementCosts', e.target.value)
                }}/>
            </div>
            }
            {(!insurance.leased || insurance.leased !== 'Yes') &&
            <>
                <div>
                    <label className="input-label">Square Footage</label>
                    <input className="input-text" value={formatNumber(insurance.squareFootage)} onChange={(e) => {
                        setNumberValue('squareFootage', e.target.value)
                    }}/>
                </div>
                <div>
                    <label className="input-label">Construction Type</label>
                    <select className="input-text"
                            value={insurance.constructionType} onChange={(e) => {
                        setValue('constructionType', e.target.value)
                    }}>
                        <option value=''>Please select a type</option>
                        <option>Frame</option>
                        <option>Masonry</option>
                        <option>Metal/Steel</option>
                        <option>Non-Combustible</option>
                    </select>
                </div>

                <div>
                    <label className="input-label">Roof Type</label>
                    <select className="input-text"
                            value={insurance.roofType} onChange={(e) => {
                        setValue('roofType', e.target.value)
                    }}>
                        <option value=''>Please select a type</option>
                        <option>Asphalt shingles</option>
                        <option>Flat</option>
                        <option>Metal</option>
                        <option>Tile</option>
                    </select>
                </div>

                <div>
                    <label className="input-label">Sprinklered</label>
                    <input className="input-text"
                           value={insurance.sprinklered} onChange={(e) => {
                        setValue('sprinklered', e.target.value)
                    }}/>
                </div>
                <div>
                    <label className="input-label">Building Replacement Costs ($)</label>
                    <input className="input-text"
                           value={formatNumber(insurance.replacementCosts)} onChange={(e) => {
                        setNumberValue('replacementCosts', e.target.value)
                    }}/>
                </div>
            </>
            }
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