import { useEffect, useState } from "react";
import { createGuid } from "../../helpers/createGuid";
import LoadingIcon from "../../components/UI/LoadingIcon/LoadingIcon";
import Items from "../../components/Items/Items";

function Home(props) {
    const [items, setItems] = useState([]);
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(true);

    const fetchItems = async () => {
        return await new Promise(function (resolve) {
            setTimeout(function () {
                setItems([
                    {
                        id: "f8137825-7146-4282-b0fa-baaf1a099a82",
                        name: "Pizza Margheritta",
                        cost: Number(25.00).toFixed(2)
                    },
                    {
                        id: "5daa2333-ba84-460b-adc1-a66f983bf908",
                        name: "Pizza Funghi",
                        cost: Number(29.25).toFixed(2)
                    },
                    {
                        id: "20b2a65a-f009-4aa2-a4ca-db35ff141cb0",
                        name: "Pizza Capriciosa",
                        cost: Number(30.25).toFixed(2)
                    },
                ])
                setError('');
                setLoading(false);
            }, 500);
        });
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