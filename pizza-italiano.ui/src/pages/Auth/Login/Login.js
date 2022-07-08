import axios from "../../../axios-setup";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { info } from "../../../components/notifications";
import LoadingButton from "../../../components/UI/LoadingButton/LoadingButton";
import useAuth from "../../../hooks/useAuth";

function Login() {
    const [auth, setAuth] = useAuth();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const submit = async (event) => {
        event.preventDefault();
        setLoading(true);
        
        try {
            const response = await axios.post('identity/sign-in', {
                email,
                password
            });
            debugger;
            setAuth({
                accessToken: response.data.accessToken,
                refreshToken: response.data.refreshToken,
                role: response.data.role,
                expires: response.data.expires
            });
            info("Successfully signed in", true)
            navigate('/');
        } catch (exception) {
            console.log(exception);
            setError(exception.response.data.reason);
            setLoading(false);
        }
    }

    useEffect(() => {
        if (auth) {
            navigate('/');
        }
    }, []);

    return (
        <div>
            <h2>Logowanie</h2>

            <form onSubmit={submit}>
                <div className="form-group">
                    <label htmlFor="email-input" >Email</label>
                    <input name="email" 
                        id="email-input"
                        value={email} 
                        onChange={event => setEmail(event.target.value)} 
                        type="email" 
                        className="form-control" />
                </div>
                <div className="form-group">
                    <label htmlFor="password-input">Password</label>
                    <input id="password-input"
                        name="password" 
                        value={password} 
                        onChange={event => setPassword(event.target.value)} 
                        type="password" 
                        className="form-control" />
                </div>

                {error ? (
                    <div className="alert alert-danger">
                        {error}
                    </div>
                ) : null}

                <LoadingButton loading={loading} >Login</LoadingButton>
            </form>
        </div>
    )
}

export default Login;