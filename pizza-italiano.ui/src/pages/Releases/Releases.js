import { useEffect, useState } from "react";
import LoadingIcon from "../../components/UI/LoadingIcon/LoadingIcon";
import { createGuid } from "../../helpers/createGuid";

export function Releases() {
    const [loading, setLoading] = useState(true);
    const [releases, setReleases] = useState([]);
    const [error, setError] = useState('');
    
    const fetchReleases = async () => {
        return await new Promise(function (resolve) {
            setTimeout(function () {
                setReleases([
                    {
                        id: createGuid(),
                        orderId: createGuid(),
                        orderProductId: createGuid(),
                        date: new Date().toLocaleString()
                    }
                ])
                setLoading(false);
                setError('');
            }, 500);
        });
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
                    <table className="table table-striped">
                        <thead>
                            <tr>
                                <th scope="col">OrderId</th>
                                <th scope="col">OrderProductId</th>
                                <th scope="col">Create date</th>
                                <th scope="col">Action</th>
                            </tr>
                        </thead>
                        <tbody>
                            {releases.map(p => (
                                <tr id ={p.id} key={new Date().getTime() + Math.random() + Math.random() + p.id}>
                                    <td>{p.orderId}</td>
                                    <td>{p.orderProductId}</td>
                                    <td>{p.date}</td>
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