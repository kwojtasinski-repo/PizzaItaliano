import { useNavigate, useParams } from "react-router-dom";
import ProductForm from "./ProductForm";

function EditProduct(props) {
    const { id } = useParams();
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

export default EditProduct;