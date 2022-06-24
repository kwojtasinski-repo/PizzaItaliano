import { useEffect, useState } from "react";
import { NavLink } from "react-router-dom";
import LoadingIcon from "../../components/UI/LoadingIcon/LoadingIcon";
import { createGuid } from "../../helpers/createGuid";

export function Payments() {
    const [loading, setLoading] = useState(true);
    const [payments, setPayments] = useState([]);
    const [error, setError] = useState('');

    const fetchPayments = async () => {
        return await new Promise(function (resolve) {
            setTimeout(function () {
                setPayments([
                    {
                        id: createGuid(),
                        paymentNumber: createGuid(),
                        cost: Number(250.00).toFixed(2),
                        orderId: createGuid(),
                        createDate: new Date().toLocaleString(),
                        modifiedDate: new Date().toLocaleString(),
                        paymentStatus: "Paid",
                        paid: true
                    }
                ])
                setLoading(false);
                setError('');
            }, 500);
        });
    }

    useEffect(() => {
        fetchPayments();
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
                                <th scope="col">Action</th>
                            </tr>
                        </thead>
                        <tbody>
                            {payments.map(p => (
                                <tr id ={p.id} key={new Date().getTime() + Math.random() + Math.random() + p.id}>
                                    <td>{p.paymentNumber}</td>
                                    <td>{p.cost} USD</td>
                                    <td>{p.orderId}</td>
                                    <td>{p.createDate}</td>
                                    <td>{p.modifiedDate}</td>
                                    <td>{p.paymentStatus}</td>
                                    <td>{p.paid ? "Yes" : "No"}</td>
                                    <td><button className="btn btn-danger">Delete</button></td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                    
                </div>
            )}
        </>
    )
}