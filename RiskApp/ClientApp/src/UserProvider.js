import React, {useEffect, useState} from "react";
import UserService from "./services/UserService";
import UserContext from "./UserContext";

const UserProvider = ({children}) => {
    const [user, setUser] = useState()

    useEffect(() => {
        UserService.getUserInfo().then(userData => {
                if (!userData || (userData.status && userData.status === 401)) {
                    window.location.href = "/Account/SignIn?ReturnUrl=%2F"
                    return;
                }

                //if we got here, then the response is good, and has been JSON processed
                if (userData.roles.includes("ROLE_BROKER")) {
                    userData.broker = true;
                }
                setUser(userData)
            }
        )

    }, [])


    return (
        <UserContext.Provider value={{user}}>
            {children}
        </UserContext.Provider>
    )
}
export default UserProvider