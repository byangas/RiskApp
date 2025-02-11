
import React from 'react';
import { Link } from 'react-router-dom';
import Image from 'react-bootstrap/Image';

export default function User() {

  return (
    <span>
      <a href="/Account/SignOut" style={{marginRight:"10px"}}>Sign Out</a>
      <Link to={'/profile'}>
               <Image roundedCircle  src="/assets/user.png" className="profile-pic" />
      </Link>
    </span>
  )
}
