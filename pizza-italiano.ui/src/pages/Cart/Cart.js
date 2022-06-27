import axios from "../../axios-setup";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { deleteItemFromCart, showItemsInCart } from "../../components/Cart/Cart";
import LoadingButton from "../../components/UI/LoadingButton/LoadingButton";
import styles from "./Cart.module.css";
import { createGuid } from "../../helpers/createGuid";

function Cart(props) {
    const [items, setItems] = useState([]);
    const [loading, setLoading] = useState(false);
    const disabledButton = items.length > 0 ? false : true;
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const removeItemHandler = (id) => {
        deleteItemFromCart(id);
        navigate(0); // refresh page
    }

    useEffect(() => {
        setItems(showItemsInCart());
    }, [])

    const summaryHandler = async () => {
        debugger;
        const orderId = createGuid();
        await axios.post('/orders', { orderId });

        for (const item of items) {
            await axios.post('/orders/order-product', {
                orderId,
                orderProductId: createGuid(),
                productId: item.id,
                quantity: item.quantity
            });
        }

        navigate(`/orders/${orderId}`);
    }

    return (
        <div className={styles.cart}>
            <div className={styles.title} >
                Cart
            </div>
            {error ? (
                <div className="alert alert-danger">{error}</div>
            ) : null}
            <table className="table">
                <thead>
                    <tr>
                        <th scope="col">#</th>
                        <th scope="col">Name</th>
                        <th scope="col">Cost</th>
                        <th scope="col">Quantity</th>
                    </tr>
                </thead>
                <tbody>
                {items.map(i => (
                    <tr key={i.id} >
                        <th scope="row">{i.id}</th>
                        <td>{i.name}</td>
                        <td>{i.cost}</td>
                        <td>{i.quantity}</td>
                        <td>
                            <button className="btn btn-danger ms-2"
                                    onClick={() => removeItemHandler(i.id)} >
                                    Usuń
                            </button>
                        </td>
                    </tr> ))}        
                </tbody>
            </table>
            <div>
                <LoadingButton className="btn btn-warning mt-2 float-end"
                        style={{ marginRight: "20%" }}
                        loading={loading}
                        onClick={summaryHandler}
                        disabled={disabledButton} >
                        Summary
                </LoadingButton>
            </div>
        </div>
    )
}

export default Cart;