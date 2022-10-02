import axios from "../../axios-setup";
import { useEffect, useState } from "react";
import LoadingIcon from "../../components/UI/LoadingIcon/LoadingIcon";
import { mapToOrders } from "../../helpers/mapper";
import { NavLink } from "react-router-dom";
import Popup, { Type } from "../../components/Popup/Popup";

export default function Orders(props) {
    const [orders, setOrders] = useState([]);
    const [error, setError] = useState('');
    const [errorWhileDelete, setErrorWhileDelete] = useState('');
    const [loading, setLoading] = useState(true);
    const [isOpen, setIsOpen] = useState(false);
    const [orderToDelete, setOrderToDelete] = useState(null);

    const fetchOrders = async () => {
        try {
            const response = await axios.get('/orders');
            setOrders(mapToOrders(response.data));
        } catch(exception) {
            console.log(exception);
            setError(exception);
        }
        setLoading(false);
    };

    useEffect(() => {
        fetchOrders();
    }, [])

    const deleteOrder = (order) => {
        setOrderToDelete(order);
        setIsOpen(true);
    }
    
    const handleDeleteOrder = async () => {
        try {
            await axios.delete(`/orders/${orderToDelete.id}`);
            setErrorWhileDelete('');
            setIsOpen(false);
            fetchOrders();
        } catch(exception) {
            console.log(exception);
            setErrorWhileDelete(exception);
        }
    }

    const handleClose = () => {
        setErrorWhileDelete('');
        setIsOpen(false);
    }

    return loading ? <LoadingIcon /> : (
        <>
            {error ? (
                <div className="alert alert-danger">{error}</div>
            ) : null}
            {isOpen && <Popup handleConfirm = {handleDeleteOrder}
                                      handleClose = {handleClose}
                                      type = {Type.alert}
                                      content = {<>
                                          <p>Do you want to delete product '{orderToDelete.orderNumber}'?</p>
                                          {errorWhileDelete ? (
                                                <div className="alert alert-danger">{errorWhileDelete}</div>
                                            ) : null}
                                      </>}
                    /> }
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
                                    <NavLink to={`/orders/${order.id}`} className="btn btn-primary me-2">
                                        Details
                                    </NavLink>
                                    <NavLink to={`edit/${order.id}`} className="btn btn-warning me-2">
                                        Edit
                                    </NavLink>
                                    <button className="btn btn-danger" onClick={() => deleteOrder(order)}>
                                        Delete
                                    </button>
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