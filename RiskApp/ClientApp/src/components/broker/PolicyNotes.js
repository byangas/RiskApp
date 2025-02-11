import React, {useEffect, useState} from "react";
import Button from "react-bootstrap/Button";
import PolicyService from "../../services/PolicyService";

export default function PolicyNotes({policyId}) {

    const [notes, setNotes] = useState([])
    const [newNoteContent, setNewNoteContent] = useState("")


    useEffect(()=> {
        LoadNotes();
    },[])

    function LoadNotes() {
        PolicyService.getPolicyNotes(policyId).then(theNotes=>{
            setNotes(theNotes)
        })
    }

    function addNewNote() {
        PolicyService.addPolicyNote(policyId, newNoteContent).then(()=> {
            setNewNoteContent("")
            LoadNotes()
        })
    }

    return (
        <div>
            <div className="section-header">Marketing Notes</div>
            <div>
                {    notes.map(note => {

                    return (
                        <div key={note.id}>
                            <div className="marketing-contact-notes">
                                {note.note}
                                <div style={{fontSize: "12px", fontWeight:"bold"}}>Created By: { note.createdBy }</div>
                                <div style={{fontSize: "10px"}}>{ (new Date(note.created)).toLocaleString("medium") }</div>
                            </div>

                        </div>
                    )
                })}
            </div>

            <textarea className="marketing-contact-notes" placeholder="Add note here" className="input-text-area" lines="3"
                      value={newNoteContent}
                      onChange={(e) => setNewNoteContent(e.target.value)}> </textarea>
            <Button onClick={() => addNewNote()}>Add Note</Button>
        </div>
    )
}