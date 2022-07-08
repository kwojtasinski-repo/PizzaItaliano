import React from "react";
import AddProduct from "../pages/Product/AddProduct";
import EditProduct from "../pages/Product/EditProduct";
import ViewProduct from "../pages/Product/ViewProduct";

export function policiesAuthentication(props) {
    let role = '';

    React.Children.map(props, (child) => {
        switch(child.type) {
            case EditProduct : 
                role = 'admin';
                break;
            case AddProduct : 
                role = 'admin';
                break;
            case ViewProduct : 
                role = 'admin';
                break;
            default:
                break;
        }
    });
    
    return role;
}