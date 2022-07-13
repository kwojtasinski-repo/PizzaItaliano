import axios from "../../axios-setup";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import LoadingIcon from "../../components/UI/LoadingIcon/LoadingIcon";
import { mapToReleases } from "../../helpers/mapper";
import useAuth from "../../hooks/useAuth";

export function ReleasesByOrderId() {
    const { id } = useParams();
    const [loading, setLoading] = useState(true);
    const [releases, setReleases] = useState([]);
    const [error, setError] = useState('');
    const [auth] = useAuth();

    const fetchReleases = async () => {
        try {
            const response = await axios.get(`/releases/by-order/${id}`);
            setReleases(mapToReleases(response.data));
        } catch(exception) {
            console.log(exception);
            setError(exception.response?.data?.reason)
        }
        
        setLoading(false);
    }

    useEffect(() => {
        fetchReleases();
    }, [])

    return (
        <>
            {loading ? <LoadingIcon /> : (
                <div className="pt-2">
                    {error ? (
                        <div className="alert alert-danger">{error}</div>
                    ) : null}
                    <h2>Releases for order with id: {id}</h2>
                    <table className="table table-striped">
                        <thead>
                            <tr>
                                <th scope="col">OrderId</th>
                                <th scope="col">OrderProductId</th>
                                <th scope="col">Create date</th>
                                {auth.role === 'admin' ?
                                    <th scope="col">Action</th>
                                : null}
                            </tr>
                        </thead>
                        <tbody>
                            {releases.map(p => (
                                <tr id ={p.id} key={new Date().getTime() + Math.random() + Math.random() + p.id}>
                                    <td>{p.orderId}</td>
                                    <td>{p.orderProductId}</td>
                                    <td>{p.date}</td>
                                    {auth.role === 'admin' ?
                                        <td><button className="btn btn-danger">Delete</button></td>
                                    : null}
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </>
    )
}