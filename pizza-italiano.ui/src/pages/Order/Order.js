import axios from "../../axios-setup";
import { useEffect, useState } from "react";
import { NavLink, useParams } from "react-router-dom";
import LoadingIcon from "../../components/UI/LoadingIcon/LoadingIcon";
import styles from './Order.module.css'
import { mapToOrder } from "../../helpers/mapper";

function Order(props) {
    const { id } = useParams();
    const [order, setOrder] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    const fetchCart = async () => {
        try {
            const response = await axios.get(`orders/${id}`);
            setOrder(mapToOrder(response.data));
        } catch(exception) {
            console.log(exception);
            setError(exception.response?.data?.reason)
        }
        
        setLoading(false);
    }

    useEffect(() => {
        fetchCart();
    }, [])

    const removeOrderProductHandler = async (id) => {
        
    }

    const setOrderAsReady = () => {
        setOrder({...order, orderStatus: "Ready"});
    }

    const payForOrder = () => {
        setOrder({...order, orderStatus: "Paid"});
    }

    const releaseOrder = () => {
        setOrder({...order, orderStatus: "Released"});
    }

    return (
        loading ? <LoadingIcon /> : (
            <>
            {error ? (
                <div className="alert alert-danger">{error}</div>
            ) : null}
            {order ? 
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
                            <td>{oi.itemName}</td>
                            <td>{oi.quantity}</td>
                            <td>{oi.cost} USD</td>
                            <td>{oi.orderProductStatus}</td>
                            <td>
                                <NavLink to = {`/${oi.productId}`}>
                                    <button className="btn btn-primary">Show item</button>
                                </NavLink>
                                <button className="btn btn-danger ms-2"
                                        onClick={() => removeOrderProductHandler(oi.id)} >
                                        Delete
                                </button>
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
                            'New': <button className="btn btn-success mt-2 float-end" onClick={setOrderAsReady}>
                                        Set as ready
                                    </button>,
                            'Ready': <button className="btn btn-success mt-2 float-end" onClick={payForOrder}>
                                        Pay
                                     </button>,
                            'Paid': (
                                        <>
                                            <button className="btn btn-success mt-2 float-end" onClick={releaseOrder}>
                                                Release
                                            </button>
                                            <NavLink className={"btn btn-primary"}
                                                    to={`/`} >
                                                        Show payment
                                            </NavLink>
                                        </>
                                    ),
                            'Released': <>
                                            <NavLink className={"btn btn-primary me-2"}
                                                    to={`/`} >
                                                        Show payment
                                            </NavLink>
                                            <NavLink className={"btn btn-primary"}
                                                    to={`/`} >
                                                        Show releases
                                            </NavLink>
                                        </>
                        }[order.orderStatus]
                    }
                </div>
            </div>
        :  <h4>Order with id: {id} not found</h4>}
        </>
    ))
}

export default Order;