import axios from "../../axios-setup";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { mapToUser } from "../../helpers/mapper";
import LoadingIcon from "../../components/UI/LoadingIcon/LoadingIcon";
import LoadingButton from "../../components/UI/LoadingButton/LoadingButton";
import Input from "../../components/Input/Input";
import { validate } from "../../helpers/validation";

function EditUser(props) {
    const { id } = useParams();
    const navigate = useNavigate();
    const [user, setUser] = useState();
    const [loading, setLoading] = useState(true);
    const [loadingData, setLoadingData] = useState(false);
    const [form, setForm] = useState({
        role: {
            value: '',
            error: '',
            showError: false,
            rules: []
        }
    });
    const [error, setError] = useState('');
    
    const fetchUser = async() => {
        try {
            const response = await axios.get(`/identity/users/${id}`);
            const userFromApi = mapToUser(response.data);
            setUser(userFromApi);
            setForm({
                ...form,
                role: {
                    ...form.role,
                    value: userFromApi.role
                }
            });
            setLoading(false);
        } catch(exception) {
            console.log(exception);
            setError(exception);
        }
    }

    const buttonDisabled = Object.values(form.role.error).filter(x => x).length;

    const submit = async (event) => {
        setError('');
        event.preventDefault();
        setLoadingData(true);
        
        try {
            const data = {
                id,
                role: form.role.value,
            };

            await axios.put(`identity/users/${id}/change-role`, data);
            navigate(`/user-management`);
        } catch (exception) {            
            setError(exception);
            setLoadingData(false);
        }
    };

    const changeHandler = (value, fieldName) => {
        const error = validate(form[fieldName].rules, value);

        setForm({
            ...form,
            [fieldName]: {
                ...form[fieldName],
                value,
                showError: true,
                error
            }
        });
    };
    
    useEffect(() => {
        fetchUser();
    }, [])

    return (
        <>
        {loading ? 
            <>
                <LoadingIcon />
                {error ? (
                        <div className="alert alert-danger">{error}</div>
                    ) : null}
            </> :
        <form onSubmit={submit}>

                <div className="form-group">
                    <label>Email</label>
                    <input name="email" 
                           value={user.email}
                           type="email" 
                           className={`form-control`} 
                           readOnly />
                </div>
                <div className="form-group">
                    <Input label = "Role"
                           type = "select"
                           options = { 
                            [
                                {   
                                    key: 'admin',
                                    value: 'admin',
                                    label: 'admin'
                                },
                                {   
                                    key: 'user',
                                    value: 'user',
                                    label: 'user'
                                }
                            ]
                           } 
                           value = {form.role.value}
                           onChange = {val => changeHandler(val, 'role')}
                           error = {form.role.error}
                           showError = {form.role.showError} />
                </div>
                
                {error ? (
                    <div className="alert alert-danger">{error}</div>
                ) : null}

                <LoadingButton loading={loadingData}
                        disabled={buttonDisabled} >Save</LoadingButton>
        </form>
        }
        </>
    )
}

export default EditUser;