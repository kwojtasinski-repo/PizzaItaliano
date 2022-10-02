import axios from "../../axios-setup";
import { useEffect, useState } from "react";
import { NavLink, useParams } from "react-router-dom";
import LoadingIcon from "../../components/UI/LoadingIcon/LoadingIcon";
import { mapToOrder } from "../../helpers/mapper";
import styles from './Order.module.css'
import useAuth from "../../hooks/useAuth";
import AddPosition from "./AddPosition/AddPosition";
import { createGuid } from "../../helpers/createGuid";

function EditOrder(props) {
    const { id } = useParams();
    const [auth] = useAuth();
    //const navigate = useNavigate();
    const [loading, setLoading] = useState(true);
    const [order, setOrder] = useState(null);
    const [error, setError] = useState('');
    const [addPositionToOrder, setAddPositionToOrder] = useState(false);
    
    const fetchOrder = async () => {
        try {
            const response = await axios.get(`/orders/${id}`);
            setOrder(mapToOrder(response.data));
        } catch(exception) {
            console.log(exception);
            setError(exception);
        }
        
        setLoading(false);
    }

    useEffect(() => {
        fetchOrder();
    }, [])

    const deletePosition = async (orderItemId) => {
        try {
            await axios.delete(`/orders/${id}/order-product/${orderItemId}/quantity/1`);
            fetchOrder();
        } catch(exception) {
            console.log(exception);
            setError(exception);
        }
    }

    const addPosition = () => {
        setAddPositionToOrder(true);
    }

    const cancelAddPosition = () => {
        setAddPositionToOrder(false);
    }

    
    const setOrderAsReady = async () => {
        try {
            await axios.put(`/orders/${id}`, {
                orderId: id
            });
            fetchOrder();
        } catch(exception) {
            console.log(exception);
            setError(exception);
        }
    }

    const payForOrder = async() => { 
        try {
            await axios.put(`/payments/${id}/pay`, {
                orderId: id
            });
            fetchOrder();
        } catch(exception) {
            console.log(exception);
            setError(exception);
        }
    }

    const setAsNew = async () => {
        try {
            await axios.put(`/orders/${id}/new`, {
                orderId: id
            });
            fetchOrder();
        } catch(exception) {
            console.log(exception);
            setError(exception);
        }
    }

    const changeFromPaidToReady = async () => {
        try {
            const response = await axios.get(`payments/by-order/${id}`);
            await axios.put(`/payments/${response.data.id}`, {
                paymentId: response.data.id,
                paymentStatus: 'Unpaid'
            });
            fetchOrder();
        } catch(exception) {
            console.log(exception);
            setError(exception);
        }
    }

    return (
        loading ? <LoadingIcon /> : (
            <>
                {error ? (
                    <div className="alert alert-danger">{error}</div>
                ) : null}
                <div className={styles.cart}>
                    <div className={styles.title} >
                        Order number: {order.orderNumber}
                    </div>
                    {addPositionToOrder ? <AddPosition orderId={order.id} cancelAddPosition={cancelAddPosition} /> : null }
                    <div className="mt-2 mb-2">
                        {order.orderStatus.toLowerCase() === 'new' && addPositionToOrder === false ? (
                            <button className="btn btn-primary me-2" onClick={addPosition}>
                                Add Position
                            </button>
                        ) : null}
                        {order.orderStatus.toLowerCase() === 'new' && addPositionToOrder === false ? (
                            <button className="btn btn-success me-2" onClick={setOrderAsReady}>
                                            Set as ready
                                        </button>
                        ) : null}
                        {order.orderStatus.toLowerCase() === 'ready' ? (
                            <button className="btn btn-success me-2" onClick={setAsNew}>
                                Set as new
                            </button>
                        ) : null}
                        {order.orderStatus.toLowerCase() === 'ready' ? (
                            <button className="btn btn-success me-2" onClick={payForOrder}>
                                Pay
                            </button>
                        ) : null}
                        {order.orderStatus.toLowerCase() === 'paid' ? (
                            <button className="btn btn-success me-2" onClick={changeFromPaidToReady}>
                                Set as ready
                            </button>
                        ) : null}
                    </div>
                    <table className="table">
                        <thead>
                            <tr>
                                <th scope="col">#</th>
                                <th scope="col">Order number</th>
                                <th scope="col">Status</th>
                                <th scope="col">Order Date</th>
                                <th scope="col">Release Date</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr key = {order.id} >
                                <th scope="row">{order.id}</th>
                                <td>{order.orderNumber}</td>
                                <td>{order.orderStatus}</td>
                                <td>{order.orderDate}</td>
                                <td>{order.ReleaseDate}</td>
                            </tr>
                        </tbody>
                    </table>
                    <table className="table">
                        <thead>
                            <tr>
                                <th scope="col">#</th>
                                <th scope="col">Name</th>
                                <th scope="col">Quantity</th>
                                <th scope="col">Cost</th>
                                <th scope="col">Status</th>
                                <th scope="col">Action</th>
                            </tr>
                        </thead>
                        <tbody>
                        {order.orderProducts ? (order.orderProducts.map(oi => (
                            <tr key = {oi.id} >
                                <th scope="row">{oi.id}</th>
                                <td>{oi.productName}</td>
                                <td>{oi.quantity}</td>
                                <td>{oi.cost} USD</td>
                                <td>{oi.orderProductStatus}</td>
                                <td>
                                    <NavLink to = {`/${oi.productId}`}>
                                            <button className="btn btn-primary me-2">Show item</button>
                                    </NavLink>
                                    {order.orderStatus.toLowerCase() === 'new' ? (
                                        <button className="btn btn-danger" onClick={() => deletePosition(oi.id)} >
                                            Delete
                                        </button>
                                    ) : null}
                                </td>
                            </tr> ))) : <></>}
                        </tbody>
                    </table>
                    <div>
                        <table className="table w-25" style={{marginLeft: 'auto', marginRight: '0'}}>
                            <thead>
                                <tr>
                                    <th>Summary: </th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr key = {new Date().getTime()}>
                                    <td>{order.cost} USD</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </>
        )
    )
}

export default EditOrder;