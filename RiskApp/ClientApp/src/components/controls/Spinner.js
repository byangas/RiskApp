import React from 'react'
import {Spinner} from 'react-bootstrap'

export default function MySpinner() {
    return (
        <div style={{margin:"10px"}} className="totally-centered">
            <Spinner animation="border" variant="primary"/> Loading...
        </div>
    )
}