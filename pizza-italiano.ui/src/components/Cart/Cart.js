const key = 'cart';
let cart = JSON.parse(window.localStorage.getItem(key)) ?? [];

export function showItemsInCart() {
    return cart;
}

export function addItemToCart(item) {
    const itemExists = cart.find(i => i.id === item.id);
    let newItem = {...item, quantity: 1 };
    let newCart = [];
    
    if (itemExists) {
        newItem.quantity = itemExists.quantity + 1;
        newCart = cart.filter(c => c.id !== newItem.id);
        newCart = [...newCart, newItem];
    } else {
        newCart = [...cart, newItem];
    }

    window.localStorage.setItem(key, JSON.stringify(newCart));
}

export function deleteItemFromCart(id) {
    const itemExists = cart.find(i => i.id === id);
        
    if (itemExists) {
        let newCart = [...cart];
        itemExists.quantity = itemExists.quantity - 1;
        
        if (itemExists.quantity > 0) {
            newCart = cart.filter(c => c.id !== id);
            newCart = [...newCart, itemExists];
        } else {
            newCart = cart.filter(c => c.id !== id);
        }

        window.localStorage.setItem(key, JSON.stringify(newCart));
    }
}

export function clearCart() {
    window.localStorage.removeItem(key);
}