import { useEffect, useState } from "react";
import LoadingIcon from "../../components/UI/LoadingIcon/LoadingIcon";
import Items from "../../components/Items/Items";
import { mapToProducts } from "../../helpers/mapper";
import sendHttpRequest from "../../helpers/useHttpClient";

function Home(props) {
    const [items, setItems] = useState([]);
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(true);

    const fetchItems = async () => {
        const [data, error] = await sendHttpRequest('/products', 'GET');
        setLoading(false);

        if (error) {
            // TODO: Interceptor global handling exceptions depend on status code etc and return only messages as exception so it will be easy to pass into component
            setError(error);
            return;
        }
        setItems(mapToProducts(data));
    };

    useEffect(() => {
        fetchItems();
    }, [])

    return loading ? <LoadingIcon /> : (
        <>
            {error ? (
                <div className="alert alert-danger">{error}</div>
            ) : null}
            <Items items={items} />
        </>
    )
}

export default Home;