import axios from "../../axios-setup";
import { useEffect, useState } from "react";
import { NavLink, useParams } from "react-router-dom";
import LoadingIcon from "../../components/UI/LoadingIcon/LoadingIcon";

function ViewProduct() {
    const { id } = useParams();
    const [error, setError] = useState('');
    const [product, setProduct] = useState(null);
    const [loading, setLoading] = useState(true);

    const fetchProduct = async () => {
        try {
            const response = await axios.get(`products/${id}`);
            setProduct(response.data);
        } catch(exception) {
            console.log(exception);
            setError(exception.response?.data?.reason)
        }
        
        setLoading(false);
    }

    useEffect(() => {
        fetchProduct();
    }, [])

    return (
        loading ? <LoadingIcon /> : 
        <>
            {error ? (
                <div className="alert alert-danger">{error}</div>
            ) : null}
            <form >
                <div>
                    <input label = "Name"
                        type = "text"
                        value = {product.name}
                        className={`form-control`}
                        readOnly />
                </div>
                <div className="mt-2">
                    <input label = "Cost"
                        type = "number"
                        value = {product.cost} 
                        className={`form-control`}
                        readOnly />
                </div>

                <div className="text-end mt-2">
                    <NavLink className="btn btn-primary" to = {`/products/edit/${id}`} >Edit</NavLink>
                </div>
            </form>
        </>
    )
}

export default ViewProduct;