import {defaultFormOptions, defaultOptions, jsonResponseHandler, standardCatch} from "./defaults";

export default class MessageService {
    static getMessageThread(subjectId) {
        return fetch('/api/message/subject/' + subjectId, defaultOptions).then(jsonResponseHandler);
    }

    static send(recipientId, subjectId, messageContent) {
        const formData = new FormData();
        formData.append("recipientId", recipientId);
        formData.append("subjectId", subjectId)
        formData.append("message", messageContent)
        console.log(JSON.stringify(formData))
        return fetch('/api/message/', {
            method: "post",
            body: new URLSearchParams([...formData]),
            ...defaultFormOptions
        }).then(jsonResponseHandler)
            .catch(standardCatch);
    }

    static getUnreadMessages() {
        return fetch('/api/message/unread', {
            method: "get",
            ...defaultOptions
        }).then(jsonResponseHandler)
            .catch(standardCatch);
    }

    static markAsRead(messageId) {
        return fetch('/api/message/read/' + messageId, {
            method: "put",
            ...defaultOptions
        }).then(jsonResponseHandler)
            .catch(standardCatch);

    }

    static reply(messageId, replyContent) {
        const formData = new FormData();
        formData.append("content", replyContent);
        return fetch('/api/message/reply/' + messageId, {
            method: "post",
            ...defaultFormOptions,
            body: new URLSearchParams([...formData])
        }).then(jsonResponseHandler)
            .catch(standardCatch);

    }
}