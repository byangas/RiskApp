import React from 'react';
import Image from 'react-bootstrap/Image';
import {Link, NavLink} from 'react-router-dom';
import User from './User';
import Spinner from '../components/controls/Spinner'


export default function Navigation({ userRoles }) {

    if (!userRoles) {
        return <Spinner />
    }

    return (
        <div className="header">
                <span>
                    <Link to={'/'}>
                        <Image className="logoImg" src="/assets/logo_orange.svg" />
                    </Link>
                    {userRoles.broker &&
                    <>
                        <NavLink activeStyle={{fontWeight:"bold"}} style={{margin: "6px"}}  to={'/broker/contacts'}><img className='nav-icon' src="/assets/icons/contacts.svg" />Carrier Contacts </NavLink>&nbsp;&nbsp;
                        <NavLink activeStyle={{fontWeight:"bold"}} to={'/broker/customer'}>
                            <img className='nav-icon' src="/assets/icons/hands-helping-solid.svg" />
                            Customers </NavLink>
                        <NavLink activeStyle={{fontWeight:"bold"}} to={'/user/forms/'}>
                        <img className='nav-icon' src="/assets/icons/docs.svg" />
                        Supplementals </NavLink>
                    </>
                    }
                    <NavLink activeStyle={{fontWeight:"bold"}} to={'/user/messages/'}>
                            <img className='nav-icon' src="/assets/icons/message.svg" />
                            Messages </NavLink>
                </span>

            <User />
        </div>
    );

}

