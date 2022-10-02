import axios from "../../../axios-setup";
import { useEffect, useState } from "react";
import { mapToProducts } from "../../../helpers/mapper";
import LoadingIcon from "../../../components/UI/LoadingIcon/LoadingIcon";
import { createGuid } from "../../../helpers/createGuid";
import { useNavigate } from "react-router-dom";

export default function AddPosition(props) {
    const id = useState(props.orderId ? props.orderId : '00000000-0000-0000-0000-000000000000');
    const [items, setItems] = useState([]);
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    const fetchItems = async () => {
        try {
            const response = await axios.get('/products');
            setItems(mapToProducts(response.data));
        } catch(exception) {
            console.log(exception);
            setError(exception);
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
            navigate(`/orders/${id}`);
        } catch(exception) {
            console.log(exception);
            setError(exception);
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
            <button className="btn btn-primary mb-2" type="button" onClick={props.cancelAddPosition}>Cancel</button>
        </>
    )
}