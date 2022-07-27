import axios from "../../axios-setup";
import { useEffect, useState } from "react";
import LoadingIcon from "../../components/UI/LoadingIcon/LoadingIcon";
import { mapToPayments } from "../../helpers/mapper";

export function Payments() {
    const [loading, setLoading] = useState(true);
    const [payments, setPayments] = useState([]);
    const [error, setError] = useState('');

    const fetchPayment = async () => {
        try {
            const response = await axios.get(`/payments/`);
            setPayments(mapToPayments(response.data));
        } catch(exception) {
            console.log(exception);
            setError(exception);
        }
        
        setLoading(false);
    }

    //TODO: Delete payment

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
                                <th scope="col">UserId</th>
                                <th scope="col">Action</th>
                            </tr>
                        </thead>
                        <tbody>
                            {payments.map(payment =>
                                <tr id ={payment.id} key={new Date().getTime() + Math.random() + Math.random() + payment.id}>
                                    <td>{payment.paymentNumber}</td>
                                    <td>{payment.cost} USD</td>
                                    <td>{payment.orderId}</td>
                                    <td>{payment.createDate}</td>
                                    <td>{payment.modifiedDate}</td>
                                    <td>{payment.paymentStatus}</td>
                                    <td>{payment.paid ? "Yes" : "No"}</td>
                                    <td>{payment.userId}</td>
                                    <td><button className="btn btn-danger">Delete</button></td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                    
                </div>
            )}
        </>
    )
}