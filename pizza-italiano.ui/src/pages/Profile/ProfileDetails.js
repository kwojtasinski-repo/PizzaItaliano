import axios from "../../axios-setup";
import { useEffect, useState } from "react";
import LoadingButton from "../../components/UI/LoadingButton/LoadingButton";
import useAuth from "../../hooks/useAuth";
import { success as successNotification } from "../../components/notifications/index"
import { mapToUser } from "../../helpers/mapper";
import LoadingIcon from "../../components/UI/LoadingIcon/LoadingIcon";

function ProfileDetails(props) {
    const [auth, setAuth] = useAuth();
    const [user, setUser] = useState();
    const [password, setPassword] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [newPasswordConfirm, setNewPasswordConfirm] = useState('');
    const [loading, setLoading] = useState(false);
    const [loadingData, setLoadingData] = useState(true);
    const [errors, setErrors] = useState({
        email: '',
        password: '',
        newPassword: '',
        newPasswordConfirm: '',
    });
    const [error, setError] = useState('');
    const [success, setSuccess] = useState(false);

    const fetchUser = async() => {
        const response = await axios.get('/identity/me');
        setUser(mapToUser(response.data));
        setLoadingData(false);
    }

    const buttonDisabled = Object.values(errors).filter(x => x).length;

    const submit = async (event) => {
        setError('');
        setSuccess(false);
        event.preventDefault();
        setLoading(true);
        
        try {
            const data = {
                email: user.email,
                oldPassword: password,
                newPassword: newPassword,
                newPasswordConfirm: newPasswordConfirm
            };

            await axios.post('/identity/change-password', data);
            successNotification("Data login changed successfullly", true);
            setSuccess(true);
        } catch (exception) {            
            setError(exception.response.data.reason);
            setLoading(false);
        }

        setPassword('');
        setNewPassword('');
        setNewPasswordConfirm('');
        setLoading(false);
    };
    
    useEffect(() => {
        fetchUser();
    }, [])

    return (
        <>
        {loadingData ? <LoadingIcon /> :
        <form onSubmit={submit}>
            {success ? (
                <div className="alert alert-success">Data changed successfully</div>
            ) : null}

                <div className="form-group">
                    <label>Email</label>
                    <input name="email" 
                           value={user.email}
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
                           value={password}
                           onChange={event => setPassword(event.target.value)} 
                           type="password" 
                           className={`form-control ${errors.password ? 'is-invalid' : ''}`} />
                    <div className="invalid-feedback">
                        {errors.password}
                    </div>
                </div>
                <div className="form-group">
                    <label>New Password</label>
                    <input name="newPassword" 
                           value={newPassword}
                           onChange={event => setNewPassword(event.target.value)} 
                           type="password" 
                           className={`form-control ${errors.password ? 'is-invalid' : ''}`} />
                    <div className="invalid-feedback">
                        {errors.password}
                    </div>
                </div>
                <div className="form-group">
                    <label>Confirm New Password</label>
                    <input name="confirmNewPassword" 
                           value={newPasswordConfirm}
                           onChange={event => setNewPasswordConfirm(event.target.value)} 
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
        }
        </>
    )
}

export default ProfileDetails;