import { Navigate } from "react-router-dom";
import { error } from "../components/notifications";
import { policiesAuthentication } from "../helpers/policiesAuthentication";
import useAuth from "../hooks/useAuth";
import NotFound from "../pages/404/NotFound";

const RequireAuth = ({ children }) => {
    const [auth, setAuth] = useAuth();

    const role = policiesAuthentication(children);
    let response = auth ? children : (<Navigate to="/login" />);
    const currentDate = new Date();

    if (auth) {
        const tokenExpiresDate = new Date(auth.expires * 1000); // missing ms in date so multiply by 1000
        
        if (tokenExpiresDate < currentDate) {
            setAuth();
            error("Your access expired, please sign in", true);
            return (<Navigate to = "/login" />);
        }
    }

    if (auth && role) {
        response = auth.role === role ? children : (<NotFound/>);
    }

    return response;
}

export default RequireAuth