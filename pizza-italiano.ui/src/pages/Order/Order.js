import axios from "../../axios-setup";
import { useEffect, useState } from "react";
import { NavLink, Outlet, useNavigate, useParams } from "react-router-dom";
import LoadingIcon from "../../components/UI/LoadingIcon/LoadingIcon";
import styles from './Order.module.css'
import { mapToOrder } from "../../helpers/mapper";
import { createGuid } from "../../helpers/createGuid";
import useAuth from "../../hooks/useAuth";

function Order(props) {
    const { id } = useParams();
    const [order, setOrder] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [actions, setActions] = useState([]);
    const navigate = useNavigate();
    const [auth] = useAuth();

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

    const removeOrderProductHandler = async (orderItemId) => {
        try {
            await axios.delete(`/orders/${id}/order-product/${orderItemId}/quantity/1`);
            navigate(0);
        } catch(exception) {
            console.log(exception);
            setError(exception);
        }
    }

    const setOrderAsReady = async () => {
        try {
            await axios.put(`/orders/${id}`, {
                orderId: id
            });
            navigate(0);
        } catch(exception) {
            console.log(exception);
            setError(exception);
        }
    }

    const payForOrder = async() => { 
        try {
            await axios.put(`/payments/${order.id}/pay`, {
                orderId: order.id
            });
            navigate(0);
        } catch(exception) {
            console.log(exception);
            setError(exception);
        }
    }

    const releaseOrderProduct = async (orderProductId) => {
        const releaseId = createGuid();
        try {
            await axios.post(`/releases`, {
                releaseId,
                orderId: id,
                orderProductId
            });
            navigate(0);
        } catch(exception) {
            console.log(exception);
            setError(exception);
        }
    }

    const addAction = (action) => {
        if (actions.length < 5) {
            const actionsToModify = [...actions];
            actionsToModify.push(action);
            setActions(actionsToModify);
            return;
        }

        const actionToDelete = actions.find(a => true);
        const actionsModified = actions.filter(a => a !== actionToDelete);
        actionsModified.push(action);
        setActions(actionsModified);
    }

    return (
        loading ? <LoadingIcon /> : (
            <>
            {error ? (
                <div className="alert alert-danger">{error}</div>
            ) : null}
            <div className="pt-4">
                <Outlet context={{ addAction }} />    
            </div>
            {order ? 
            <div>
                {order.orderStatus === 'new' ?
                    <div>
                        <NavLink className="btn btn-primary" end to={`add-product`}>Add Product to Order</NavLink>
                    </div>
                : null}
                <div className={styles.cart}>
                    <div className={styles.title} >
                        Order number: {order.orderNumber}
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
                                    {auth.role === "admin" ?
                                        <NavLink to = {`/${oi.productId}`}>
                                            <button className="btn btn-primary">Show item</button>
                                        </NavLink>
                                    : null}
                                    {order.orderStatus === 'new' ?
                                        <button className="btn btn-danger ms-2"
                                                onClick={() => removeOrderProductHandler(oi.id)} >
                                                Delete
                                        </button>
                                    : null}
                                    {order.orderStatus === 'paid' && oi.orderProductStatus !== 'released' ?
                                        <button className="btn btn-warning ms-2"
                                                onClick={() => releaseOrderProduct(oi.id)} >
                                                Release
                                        </button>
                                    : null}
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
                    <div>
                        {
                            {
                                'new': <button className="btn btn-success mt-2 float-end" onClick={setOrderAsReady}>
                                            Set as ready
                                        </button>,
                                'ready': <button className="btn btn-success mt-2 float-end" onClick={payForOrder}>
                                            Pay
                                        </button>,
                                'paid': (
                                            <>
                                                <NavLink className={"btn btn-primary me-2"}
                                                        to={`/payments/by-order/${id}`} >
                                                            Show payment
                                                </NavLink>
                                            </>
                                        ),
                                'released': <>
                                                <NavLink className={"btn btn-primary me-2"}
                                                        to={`/payments/by-order/${id}`} >
                                                            Show payment
                                                </NavLink>
                                                <NavLink className={"btn btn-primary me-2"}
                                                        to={`/releases/by-order/${id}`} >
                                                            Show releases
                                                </NavLink>
                                            </>
                            }[order.orderStatus]
                        }
                    </div>
                </div>
            </div>
        :  <h4>Order with id: {id} not found</h4>}
        </>
    ))
}

export default Order;