import React, {useEffect, useState} from 'react';
import {Container} from 'react-bootstrap';
import {useHistory} from 'react-router-dom'
import UserService from '../services/UserService';
import ProfileEdit from "./ProfileEdit";

export default function Profile() {

    const history = useHistory()
    const [initialized, setInitialized] = useState(false);
    const [profile, setProfile] = useState();

    useEffect(() => {
        UserService.getUserProfile().then(userProfile => {
            if (userProfile) {
                setProfile(userProfile);
            }
            setInitialized(true);
        });
    }, []);


    function editComplete() {
        history.goBack()
    }

    if (!initialized) {
        return (<h1>Loading...</h1>)
    }

    return (
        <Container>
            <div className="section-header">Profile</div>
            <ProfileEdit profile={profile} editComplete={editComplete}/>
        </Container>
    );
}