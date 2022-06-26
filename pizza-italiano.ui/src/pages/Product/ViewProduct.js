import axios from "../../axios-setup";
import { useEffect, useState } from "react";
import { NavLink, useNavigate, useParams } from "react-router-dom";
import LoadingIcon from "../../components/UI/LoadingIcon/LoadingIcon";
import Popup, { Type } from "../../components/Popup/Popup";
import { mapToProduct } from "../../helpers/mapper";

function ViewProduct() {
    const { id } = useParams();
    const [error, setError] = useState('');
    const [product, setProduct] = useState(null);
    const [isOpen, setIsOpen] = useState(false);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    const fetchProduct = async () => {
        try {
            const response = await axios.get(`products/${id}`);
            setProduct(mapToProduct(response.data));
        } catch(exception) {
            console.log(exception);
            setError(exception.response?.data?.reason)
        }
        
        setLoading(false);
    }

    useEffect(() => {
        fetchProduct();
    }, [])

    const closePopUp = () => {
        setIsOpen(false);
    }

    const handleDeleteProduct = async () => {
        try {
            await axios.delete(`/products/${id}`);
            navigate(`/`);
        } catch(exception) {
            console.log(exception);
            setError(exception.response.data.reason);
        }

        setIsOpen(false);
    }

    return (
        loading ? <LoadingIcon /> : 
        <>
            {error ? (
                <div className="alert alert-danger">{error}</div>
            ) : null}
            {isOpen && <Popup handleConfirm = {handleDeleteProduct}
                                      handleClose = {closePopUp}
                                      type = {Type.alert}
                                      content = {<>
                                          <p>Do you want to delete product '{product.name}'?</p>
                                      </>}
                    /> }
            {product ? 
            <form>
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
                    <NavLink className="btn btn-primary me-2" to = {`/products/edit/${id}`} >Edit</NavLink>
                    <button type="button" className="btn btn-danger" onClick={() => setIsOpen(true)}>Delete</button>
                </div>
            </form>
            : <><div>Product with id {id} not found</div></>}
        </>
    )
}

export default ViewProduct;