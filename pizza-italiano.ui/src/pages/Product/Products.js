import axios from "../../axios-setup";
import { useEffect, useState } from "react";
import LoadingIcon from "../../components/UI/LoadingIcon/LoadingIcon";
import { mapToProducts } from "../../helpers/mapper";
import { Navigate, NavLink } from 'react-router-dom';
import Popup, { Type } from "../../components/Popup/Popup";

export default function Products() {
    const [loading, setLoading] = useState(true);
    const [products, setProducts] = useState([]);
    const [error, setError] = useState('');
    const [isOpen, setIsOpen] = useState(false);
    const [productToDelete, setProductToDelete] = useState(null);

    const fetchProducts = async () => {
        setLoading(true);
        try {
            const response = await axios.get(`/products`);
            setProducts(mapToProducts(response.data));
        } catch(exception) {
            console.log(exception);
            setError(exception);
        }
        
        setLoading(false);
    }

    useEffect(() => {
        fetchProducts();
    }, [])


    const handleDeleteProduct = async () => {
        try {
            await axios.delete(`/products/${productToDelete.id}`);
            fetchProducts();
        } catch(exception) {
            console.log(exception);
            setError(exception);
        }
        setProductToDelete(null);
        setIsOpen(false);
    }

    const closePopUp = () => {
        setIsOpen(false);
    }

    const openDeleteProductPopup = (product) => {
        setProductToDelete(product);
        setIsOpen(true);
    }

    return (
        <>
            {loading ? <LoadingIcon /> : (
                <div className="pt-2">
                    {error ? (
                        <div className="alert alert-danger">{error}</div>
                    ) : null}

                    {isOpen && <Popup handleConfirm = {handleDeleteProduct}
                                      handleClose = {closePopUp}
                                      type = {Type.alert}
                                      content = {<>
                                          <p>Do you want to delete product '{productToDelete.name}'?</p>
                                      </>}
                    /> }
                    <NavLink to={`/products/add`} className={"btn btn-primary"}>
                        Add Product
                    </NavLink>
                    <table className="table table-striped">
                        <thead>
                            <tr>
                                <th scope="col">Id</th>
                                <th scope="col">Name</th>
                                <th scope="col">Cost</th>
                                <th scope="col">Action</th>
                            </tr>
                        </thead>
                        <tbody>
                            {products.map(product =>
                                <tr id ={product.id} key={new Date().getTime() + Math.random() + Math.random() + product.id}>
                                    <td>{product.id}</td>
                                    <td>{product.name}</td>
                                    <td>{product.cost} USD</td>
                                    <td>
                                        <NavLink to={`/products/edit/${product.id}`} className={"btn btn-warning me-2"}>
                                            Edit
                                        </NavLink>
                                        <button className="btn btn-danger" onClick={() => openDeleteProductPopup(product)}>Delete</button>
                                    </td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                    
                </div>
            )}
        </>
    )
}