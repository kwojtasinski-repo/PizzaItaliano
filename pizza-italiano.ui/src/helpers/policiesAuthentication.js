import React from "react";
import Orders from "../pages/Orders/Orders";
import { Payments } from "../pages/Payments/Payments";
import AddProduct from "../pages/Product/AddProduct";
import EditProduct from "../pages/Product/EditProduct";
import ViewProduct from "../pages/Product/ViewProduct";
import { Releases } from "../pages/Releases/Releases";

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
            case Releases : 
                role = 'admin';
                break;
            case Payments :
                role = 'admin';
                break;
            case Orders :
                role = 'admin';
                break;
            default:
                break;
        }
    });
    
    return role;
}