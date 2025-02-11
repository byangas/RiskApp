import {defaultOptions, jsonResponseHandler, standardCatch} from "./defaults";

class UserService {


    static getUserInfo() {
        return fetch('/api/user/info', {...defaultOptions})
            .then(jsonResponseHandler)
            .catch(standardCatch);
    }

    static getUserRoles() {
        return fetch('/api/user/roles', defaultOptions).then(jsonResponseHandler).catch(standardCatch);
    }

    //gets the current users profile
    static getUserProfile(id) {
        return fetch('/api/user/profile/', defaultOptions).then(jsonResponseHandler).catch(standardCatch)
    }



    static updateUserProfile(profile) {
        return fetch('/api/user/profile', {
            method: 'put',
            body: JSON.stringify(profile),
            headers: {
                'Content-Type': 'application/json'
            }
        });
    }

}

export default UserService