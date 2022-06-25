import axios from "../../axios-setup";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import ProductForm from "./ProductForm";
import LoadingIcon from "../../components/UI/LoadingIcon/LoadingIcon";

function EditProduct(props) {
    const { id } = useParams();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(true);
    const [item, setItem] = useState(null);
    const [error, setError] = useState('');

    const fetchItem = async () => {
        try {
            const response = await axios.get(`/products/${id}`);
            setItem(response.data);
        } catch(exception) {
            console.log(exception);
            setError(exception.response.data.reason);
        }

        setLoading(false);
    }

    const onSubmit = async (form) => {
        debugger;
        await axios.put('/products/', form);
    }

    const redirectAfterSuccess = () => {
        navigate(`/products/details/${id}`);
    }

    useEffect(() => {
        fetchItem();
    }, [])

    return (
        loading ? <LoadingIcon /> : 
        <div>
            {error ? (
                <div className="alert alert-danger">{error}</div>
            ) : null}
            <ProductForm product={item}
                      text = "Edit product"
                      onSubmit = {onSubmit}
                      cancelEditUrl = "/"
                      cancelButtonText = "Cancel"
                      buttonText = "Accept"
                      redirectAfterSuccess = {redirectAfterSuccess} />
        </div>
    )
}

export default EditProduct;