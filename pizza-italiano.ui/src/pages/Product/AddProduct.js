import axios from "../../axios-setup";
import { useNavigate } from "react-router-dom";
import { createGuid } from "../../helpers/createGuid";
import ProductForm from "./ProductForm";

function AddProduct(props) {
    const navigate = useNavigate();

    const onSubmit = async (form) => {
        form.productId = createGuid();
        await axios.post('/products', form);
    }

    const redirectAfterSuccess = () => {
        navigate(`/products`);
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