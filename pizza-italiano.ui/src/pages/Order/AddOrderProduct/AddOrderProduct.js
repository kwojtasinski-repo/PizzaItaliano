import axios from "../../../axios-setup";
import { useEffect, useState } from "react";
import { mapToProducts } from "../../../helpers/mapper";
import LoadingIcon from "../../../components/UI/LoadingIcon/LoadingIcon";
import { createGuid } from "../../../helpers/createGuid";
import { useNavigate, useOutletContext, useParams } from "react-router-dom";

export default function AddOrderProduct(props) {
    const { id } = useParams();
    const [items, setItems] = useState([]);
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(true);
    const { addAction } = useOutletContext();
    const navigate = useNavigate();

    const fetchItems = async () => {
        try {
            const response = await axios.get('/products');
            setItems(mapToProducts(response.data));
        } catch(exception) {
            console.log(exception);
            setError(exception.message);
        }
        setLoading(false);
    };

    const addItemToOrder = async (item) => {
        try {
            await axios.post('/orders/order-product', {
                orderId: id,
                orderProductId: createGuid(),
                productId: item.id,
                quantity: 1
            });
            addAction('added-new-product-to-order');
            navigate(`/orders/${id}`);
        } catch(exception) {
            console.log(exception);
            setError(exception.message);
        }
    }

    useEffect(() => {
        fetchItems();
    }, [])

    return loading ? <LoadingIcon /> : (
        <>
            {error ? (
                <div className="alert alert-danger">{error}</div>
            ) : null}
                <table className="table">
                    <thead>
                        <tr>
                            <th scope="col">#</th>
                            <th scope="col">Name</th>
                            <th scope="col">Cost</th>
                        </tr>
                    </thead>
                    <tbody>
                    {items.map(i => (
                        <tr key = {i.id} >
                            <th scope="row">{i.id}</th>
                            <td>{i.name}</td>
                            <td>{i.cost} USD</td>
                            <td>
                                <button className="btn btn-success ms-2"
                                        onClick={() => addItemToOrder(i)} >
                                        Add to order
                                </button>
                            </td>
                        </tr> )
                    )}
                    </tbody>
                </table>
            <button className="btn btn-primary mb-2" type="button" onClick={() => navigate(`/orders/${id}`)}>Cancel</button>
        </>
    )
}