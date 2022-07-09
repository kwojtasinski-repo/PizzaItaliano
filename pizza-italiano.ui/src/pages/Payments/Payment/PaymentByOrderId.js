import axios from "../../../axios-setup";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import LoadingIcon from "../../../components/UI/LoadingIcon/LoadingIcon";
import { mapToPayment } from "../../../helpers/mapper";
import useAuth from "../../../hooks/useAuth";

export function PaymentByOrderId() {
    const { id } = useParams();
    const [loading, setLoading] = useState(true);
    const [payment, setPayment] = useState([]);
    const [error, setError] = useState('');
    const [auth] = useAuth();

    const fetchPayment = async () => {
        try {
            const response = await axios.get(`/payments/by-order/${id}`);
            setPayment(mapToPayment(response.data));
        } catch(exception) {
            console.log(exception);
            setError(exception.response?.data?.reason)
        }
        
        setLoading(false);
    }

    useEffect(() => {
        fetchPayment();
    }, [])

    return (
        <>
            {loading ? <LoadingIcon /> : (
                <div className="pt-2">
                    {error ? (
                        <div className="alert alert-danger">{error}</div>
                    ) : null}

                    <table className="table table-striped">
                        <thead>
                            <tr>
                                <th scope="col">Payment number</th>
                                <th scope="col">Cost</th>
                                <th scope="col">OrderId</th>
                                <th scope="col">Create date</th>
                                <th scope="col">Modified date</th>
                                <th scope="col">Status</th>
                                <th scope="col">Paid</th>
                                {auth.role === 'admin' ?
                                    <th scope="col">Action</th>
                                    : null
                                }
                            </tr>
                        </thead>
                        <tbody>
                            <tr id ={payment.id} key={new Date().getTime() + Math.random() + Math.random() + payment.id}>
                                <td>{payment.paymentNumber}</td>
                                <td>{payment.cost} USD</td>
                                <td>{payment.orderId}</td>
                                <td>{payment.createDate}</td>
                                <td>{payment.modifiedDate}</td>
                                <td>{payment.paymentStatus}</td>
                                <td>{payment.paid ? "Yes" : "No"}</td>
                                {auth.role === 'admin' ?
                                    <td><button className="btn btn-danger">Delete</button></td>
                                : null}
                            </tr>
                        </tbody>
                    </table>
                    
                </div>
            )}
        </>
    )
}