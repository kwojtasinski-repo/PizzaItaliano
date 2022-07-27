import axios from "../../axios-setup";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import ProductForm from "./ProductForm";
import LoadingIcon from "../../components/UI/LoadingIcon/LoadingIcon";
import { mapToProduct } from "../../helpers/mapper";

function EditProduct(props) {
    const { id } = useParams();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(true);
    const [item, setItem] = useState(null);
    const [error, setError] = useState('');

    const fetchItem = async () => {
        try {
            const response = await axios.get(`/products/${id}`);
            setItem(mapToProduct(response.data));
            setLoading(false);
        } catch(exception) {
            console.log(exception);
            setError(exception);
        }

    }

    const onSubmit = async (form) => {
        await axios.put('/products/', form);
    }

    const redirectAfterSuccess = () => {
        navigate(`/products/details/${id}`);
    }

    useEffect(() => {
        fetchItem();
    }, [])

    return (
        loading ? 
            <>
                <LoadingIcon /> 
                {error ? (
                    <div className="alert alert-danger">{error}</div>
                    ) : null}
            </> : 
        <div>
            {error ? (
                <div className="alert alert-danger">{error}</div>
            ) : null}
            {item ? 
                <ProductForm product={item}
                        text = "Edit product"
                        onSubmit = {onSubmit}
                        cancelEditUrl = "/"
                        cancelButtonText = "Cancel"
                        buttonText = "Accept"
                        redirectAfterSuccess = {redirectAfterSuccess} />
                        : <><div>Product with id {id} not found</div></>
            }
        </div>
    )
}

export default EditProduct;