export function mapExceptionToResponse(exception) {
    if (!exception) {
        return genericException();
    }

    console.log(exception);

    if (!exception.response) {
        return genericException();
    }

    if (!exception.response.data) {
        return mapFromStatusToResponse(exception.response);
    }

    return exception.response.data.reason;
}

function mapFromStatusToResponse(response) {
    let message = "";

    switch (response.status) {
        case 404:
            message = "Resource not found";
            break;
        case 0:
            message = "There was problem with network";
            break;
        default:
            message = genericException();
            break;
    }

    return message;
}

function genericException() {
    return "Something bad happen";
}