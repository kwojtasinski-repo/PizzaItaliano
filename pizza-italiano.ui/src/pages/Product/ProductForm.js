import { useEffect, useState } from "react";
import { NavLink } from "react-router-dom";
import Input from "../../components/Input/Input";
import LoadingButton from "../../components/UI/LoadingButton/LoadingButton";
import { isEmpty } from "../../helpers/stringExtensions";
import { validate } from "../../helpers/validation";

export default function ProductForm(props) {
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const [form, setForm] = useState({
        id: {
            value: ''
        },
        name: {
            value: '',
            error: '',
            showError: false,
            rules: ['required', { rule: 'min', length: 3 }]
        },
        cost: {
            value: '',
            error: '',
            showError: false,
            rules: ['required', { rule: 'greaterThan', equalityValue: 0 }]
        }
    });

    const submit = async e => {
        e.preventDefault();
        const errorItem = validateBeforeSend(form, setForm);
        if (!isEmpty(errorItem)) {
            setLoading(false);
            return;
        }
        setLoading(true);
        
        try {
            await props.onSubmit({
                productId: form.id.value,
                name: form.name.value,
                cost: Number(form.cost.value)
            });
            props.redirectAfterSuccess();
        } catch (exception) {
            console.log(exception);
            setError(exception);
        }

        setLoading(false);
    };

    
    const validateBeforeSend = (form, setForm) => {
        for(let field in form) {
            const error = validate(form[field].rules, form[field].value);

            if (error) {
                setForm({...form, 
                    [field]: {
                        ...form[field],
                        showError: true,
                        error
                    }});
                setLoading(false);
                return error;
            }
        }
    }

    const changeHandler = (value, fieldName) => {
        const error = validate(form[fieldName].rules, value);
        setForm({
            ...form, 
            [fieldName]: {
                ...form[fieldName],
                value,
                showError: true,
                error: error
            } 
        });
    };

    useEffect(() => {
        const newForm = {...form};
        for (const key in props.product) {
            newForm[key].value = props.product[key];
        }

        setForm(newForm);
    }, [props.product]);

    return (
        <>
            {error ? (
                <div className="alert alert-danger">{error}</div>
            ) : null}
            <form onSubmit={submit} >
                <Input label = "Name"
                       type = "text"
                       value = {form.name.value}
                       error = {form.name.error}
                       showError = {form.name.showError}
                       onChange = {val => changeHandler(val, 'name')} />
                <Input label = "Cost"
                        type = "number"
                        value = {form.cost.value}
                        error = {form.cost.error}
                        showError = {form.cost.showError}
                        onChange = {val => changeHandler(val, 'cost')} />

                <div className="text-end mt-2">
                    <NavLink className="btn btn-secondary me-2" to = {props.cancelEditUrl} >{props.cancelButtonText}</NavLink>
                    <LoadingButton
                        loading={loading} 
                        className="btn btn-success">
                        {props.buttonText}
                    </LoadingButton>
                </div>
            </form>
        </>
    )
}