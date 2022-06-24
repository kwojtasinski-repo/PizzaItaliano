import { useParams } from "react-router-dom";

function ViewProduct() {
    const { id } = useParams();

    return (
        <div>
            ViewProduct {id}
        </div>
    )
}

export default ViewProduct;