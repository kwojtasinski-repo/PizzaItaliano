import { useEffect, useState } from "react"
import { NavLink } from "react-router-dom";
import axios from "../../axios-setup";
import LoadingIcon from "../../components/UI/LoadingIcon/LoadingIcon";
import { mapToOrders } from "../../helpers/mapper";

function MyOrders(props) {
    const [orders, setOrders] = useState([]);
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(true);

    const fetchMyOrders = async () => {
        try {
            const response = await axios.get('/orders/my');
            setOrders(mapToOrders(response.data));
        } catch(exception) {
            console.log(exception);
            setError(exception.message);
        }
        setLoading(false);
    };

    useEffect(() => {
        fetchMyOrders();
    }, [])

    return loading ? <LoadingIcon /> : (
        <>
            {error ? (
                <div className="alert alert-danger">{error}</div>
            ) : null}
            <div className="">
                <table className="table">
                    <thead>
                        <tr>
                            <th scope="col">#</th>
                            <th scope="col">Order number</th>
                            <th scope="col">Cost</th>
                            <th scope="col">Status</th>
                            <th scope="col">Order Date</th>
                            <th scope="col">Release Date</th>
                            <th scope="col">Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        {orders ? orders.map(order => 
                            <tr key = {order.id} >
                                <th scope="row">{order.id}</th>
                                <td>{order.orderNumber}</td>
                                <td>{order.cost} USD</td>
                                <td>{order.orderStatus}</td>
                                <td>{order.orderDate}</td>
                                <td>{order.ReleaseDate}</td>
                                <td>
                                    <NavLink to={`/orders/${order.id}`} className="btn btn-primary">
                                        Details
                                    </NavLink>
                                </td>
                            </tr>
                        )
                        : null}
                    </tbody>
                </table>
            </div>
        </>
    )
}

export default MyOrders;