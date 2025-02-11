import React, {useState} from 'react';
import {Button} from 'react-bootstrap';
import ContactService from "../services/ContactService";

export default function ProfileEdit({profile, editComplete, companyId, email}) {

    const [profileEdit, setProfileEdit] = useState({...profile})
    const [errorMessages, SetErrorMessages] = useState([])

    function setValue(fieldName, val) {
        const newProf = {...profileEdit};
        newProf[fieldName] = val;
        setProfileEdit(newProf);
    }

    function save() {

        const requiredFieldsMessage = []
        SetErrorMessages([]);
        if (!profileEdit.title) {
            requiredFieldsMessage.push("Title is required")

        }

        if(!profileEdit.firstName) {
            requiredFieldsMessage.push("First name is required")
        }


        if(!profileEdit.lastName) {
            requiredFieldsMessage.push("Last name is required")
        }


        if(requiredFieldsMessage.length) {
            SetErrorMessages(requiredFieldsMessage)
            alert("Please correct errors")
            return;
        }

        if (profileEdit.id) {
            ContactService.updateContact(profileEdit).then(() => {
                editComplete()
            }).catch(error => {
                alert("Error updating profile" + error);
            });
        } else {
            profileEdit.companyId = companyId;
            profileEdit.email = email;
            ContactService.createContact(profileEdit).then(() => {
                alert("Contact Created")
                editComplete()
            }).catch(error => {
                alert("Error creating contact" + error);
            });
        }
    }

    const errors = errorMessages.map((error, index) => {
        return <div className="errors" key={index}>{error}</div>
    })
    return (
        <>
            <div>
                {errors}
            </div>
            <div>
                <label className="input-label">First Name</label>
                <input className="input-text" value={profileEdit.firstName} onChange={(e) => {
                    setValue('firstName', e.target.value)
                }}/>
            </div>
            <div>
                <label className="input-label">Last Name</label>
                <input className="input-text" value={profileEdit.lastName} onChange={(e) => {
                    setValue('lastName', e.target.value)
                }}/>
            </div>


            <div>
                <label className="input-label">Primary Number </label>
                <input className="input-text" value={profileEdit.phone} onChange={(e) => {
                    setValue('phone', e.target.value)
                }}/>
            </div>

            <div>
                <label className="input-label">Mobile Number </label>
                <input className="input-text" value={profileEdit.mobilePhone} onChange={(e) => {
                    setValue('mobilePhone', e.target.value)
                }}/>
            </div>
            <div>
                <label className="input-label">Title </label>
                <input className="input-text" value={profileEdit.title} onChange={(e) => {
                    setValue('title', e.target.value)
                }}/>
            </div>
            <div>
                <label className="input-label">Specialty </label>
                <input className="input-text" value={profileEdit.specialty} onChange={(e) => {
                    setValue('specialty', e.target.value)
                }}/>
            </div>
            <Button  onClick={() => {
                save()
            }}>Save</Button>&nbsp;
            <Button variant="secondary" onClick={() => {
                editComplete()
            }}>Cancel</Button>
        </>
    );
}