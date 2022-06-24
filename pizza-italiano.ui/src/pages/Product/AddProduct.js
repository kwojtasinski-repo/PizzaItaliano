import { useNavigate } from "react-router-dom";
import ProductForm from "./ProductForm";

function AddProduct(props) {
    let id = '';
    const navigate = useNavigate();

    const onSubmit = async (form) => {
    }

    const redirectAfterSuccess = () => {
        navigate(`/products/details/${id}`);
    }

    return (
        <div>
            <ProductForm text = "Add product"
                      onSubmit = {onSubmit}
                      cancelEditUrl = "/"
                      cancelButtonText = "Cancel"
                      buttonText = "Add"
                      redirectAfterSuccess = {redirectAfterSuccess} />
        </div>
    )
}

export default AddProduct;