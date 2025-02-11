export const defaultOptions = {
    headers: {
        "accept": "application/json",
        "Content-Type": "application/json"
    },
    redirect: "error"
}

export const defaultFormOptions = {
    redirect: "error"
}

export function jsonResponseHandler(response) {
    if (response.status === 500 || response.status === 400) {
        response.text().then(body => console.log(body))
        throw 'Error from server.'
    }
    //NO CONTENT, just return empty result. we can't call "JSON()" on it because it's empty and will
    // result in an error/exception
    if (response.status === 204) {
        return Promise.resolve();
    }

    //basically, for standard JSON processing, anything other than a 2XX response is an unexpected error
    if (response.ok) {
        return response.json();
    }

    //user is not signed in
    if (response.status === 401) {
        window.location.href = "/Account/SignIn?ReturnUrl=%2F";
        return response;
    }

    // if we got here, there must be something wrong, so throw an error
    return response.text().then(text => {
        throw text;
    }).catch(err => {
        throw `Status: ${response.status}. Message ${err}`;
    })
}

export function standardCatch(error) {
    console.log(error);
    return Promise.reject(error)
}

