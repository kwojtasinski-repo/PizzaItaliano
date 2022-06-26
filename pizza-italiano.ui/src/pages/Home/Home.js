import { useEffect, useState } from "react";
import LoadingIcon from "../../components/UI/LoadingIcon/LoadingIcon";
import Items from "../../components/Items/Items";
import axios from "../../axios-setup";
import { mapToProducts } from "../../helpers/mapper";

function Home(props) {
    const [items, setItems] = useState([]);
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(true);

    const fetchItems = async () => {
        try {
            const response = await axios.get('/products');
            setItems(mapToProducts(response.data));
        } catch(exception) {
            console.log(exception);
            setError(exception.message);
        }
        setLoading(false);
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