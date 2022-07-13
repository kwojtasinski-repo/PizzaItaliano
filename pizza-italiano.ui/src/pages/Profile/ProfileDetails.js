import axios from "../../axios-setup";
import { useEffect, useState } from "react";
import LoadingButton from "../../components/UI/LoadingButton/LoadingButton";
import useAuth from "../../hooks/useAuth";
import { success as successNotification } from "../../components/notifications/index"

function ProfileDetails(props) {
    const [auth, setAuth] = useAuth();
    const [email, setEmail] = useState(auth.email);
    const [password, setPassword] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [newPasswordConfirm, setNewPasswordConfirm] = useState('');
    const [loading, setLoading] = useState(false);
    const [errors, setErrors] = useState({
        email: '',
        password: '',
        newPassword: '',
        newPasswordConfirm: '',
    });
    const [error, setError] = useState('');
    const [success, setSuccess] = useState(false);

    const buttonDisabled = Object.values(errors).filter(x => x).length;

    const submit = async (event) => {
        event.preventDefault();
        setLoading(true);
        
        try {
            const data = {
                oldEmail: auth.email,
                newEmail: email,
                oldPassword: password,
                newPassword: newPassword,
                newPasswordConfirm: newPasswordConfirm
            };

            if (password) {
                data.password = password;
            }

            const response = await axios.post('/identity', data);

            if (response.data) {
                setAuth({
                    email: response.data.email,
                    token: response.data.accessToken,
                    userId: response.data.id,
                    claims: response.data.claims
                });
                successNotification("Data login changed successfullly", true);
                setSuccess(true);
                setPassword('');
                setNewPassword('');
                setNewPasswordConfirm('');
            }            
        } catch (exception) {            
            setError(exception.response.data.reason);
            setLoading(false);
        }

        setLoading(false);
    };
    
    useEffect(() => {
    }, [])

    return (
        <>
        <form onSubmit={submit}>
            {success ? (
                <div className="alert alert-success">Data changed successfully</div>
            ) : null}

                <div className="form-group">
                    <label>Email</label>
                    <input name="email" 
                           value={email} 
                           onChange={event => setEmail(event.target.value)} 
                           type="email" 
                           className={`form-control ${errors.email ? 'is-invalid' : 'is-valid'}`} 
                           readOnly />
                    <div className="invalid-feedback">
                        {errors.email}
                    </div>
                </div>
                <div className="form-group">
                    <label>Password</label>
                    <input name="password" 
                           onChange={event => setPassword(event.target.value)} 
                           type="password" 
                           className={`form-control ${errors.password ? 'is-invalid' : ''}`} />
                    <div className="invalid-feedback">
                        {errors.password}
                    </div>
                </div>
                
                {error ? (
                    <div className="alert alert-danger">{error}</div>
                ) : null}

                <LoadingButton loading={loading}
                        disabled={buttonDisabled} >Save</LoadingButton>
        </form>
        </>
    )
}

export default ProfileDetails;