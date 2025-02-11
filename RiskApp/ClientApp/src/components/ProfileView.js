import React, { Component } from 'react';

export default function ProfileView(props) {
    const { user } = props

    if (!user) {
        return "";
    }

    return (
        <>
            <div>
                <label className="input-label">Name</label>
                <div className="input-text" >{user.name}</div>
            </div>
            <div>
                <label className="input-label">Email</label>
                <div className="input-text">{user.email}</div>
            </div>
            <div>
                <label className="input-label">Business Name</label>
                <div className="input-text">{user.company}</div>
            </div>
            <div>
                <label className="input-label">City </label>
                <div className="input-text">{user.city}</div>
            </div>

            <div>
                <label className="input-label">State </label>
                <div className="input-text">{user.state}</div>
            </div>
            <div>
                <label className="input-label">Contact Number </label>
                <div className="input-text">{user.phone}</div>
            </div>
        </>
    );

}
