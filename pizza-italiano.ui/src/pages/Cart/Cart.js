import axios from "../../axios-setup";
import { useContext, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { clearCart, deleteItemFromCart, showItemsInCart } from "../../components/Cart/Cart";
import LoadingButton from "../../components/UI/LoadingButton/LoadingButton";
import styles from "./Cart.module.css";
import { createGuid } from "../../helpers/createGuid";
import { warning } from "../../components/notifications";
import ReducerContext from "../../context/ReducerContext";

function Cart(props) {
    const [items, setItems] = useState([]);
    const [loading, setLoading] = useState(false);
    const disabledButton = items.length > 0 ? false : true;
    const navigate = useNavigate();
    const context = useContext(ReducerContext);

    const removeItemHandler = (item) => {
        deleteItemFromCart(item.id);
        warning(`Removed item '${item.name}'`, true);
        setTimeout({}, 1000);
        navigate(0); // refresh page
    }

    useEffect(() => {
        setItems(showItemsInCart());
    }, [])

    const summaryHandler = async () => {
        setLoading(true);
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

        setLoading(false);
        clearCart();
        context.dispatch({ type: "clear-cart", clearedCart: new Date() });
        navigate(`/orders/${orderId}`);
    }

    return (
        <div className={styles.cart}>
            <div className={styles.title} >
                Cart
            </div>
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
                                    onClick={() => removeItemHandler(i)} >
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