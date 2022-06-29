export function mapExceptionToResponse(exception) {
    console.log(exception);

    if (!exception) {
        return { code: "generic_exception", reason: "Something bad happen" };
    }

    if (!exception.response) {
        return { code: "generic_exception", reason: "Something bad happen" };
    }
    
    if (!exception.response.data) {
        return mapFromStatusToResponse(exception.response.status);
    }
    
    return exception.response.data;
}

function mapFromStatusToResponse(status) {
    let message = "";
    
    switch (status) {
        case 404:
            message = { code: "resource_not_found", reason: "Resource not found" };
            break;
        case 0:
            message = { code: "network_problem", reason: "There was problem with network" };
            break;
        default:
            message = { code: "generic_exception", reason: "Something bad happen" };
            break;
    }

    return message;
}