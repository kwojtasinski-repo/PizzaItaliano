const key = 'cart';
const cart = () => JSON.parse(window.localStorage.getItem(key)) ?? [];

export function showItemsInCart() {
    return cart();
}

export function getItemsInCartCount() {
    return cart().length;
}

export function addItemToCart(item) {
    const items = cart();
    const itemExists = items.find(i => i.id === item.id);
    let newItem = {...item, quantity: 1 };
    let newCart = [];
    
    if (itemExists) {
        newItem.quantity = itemExists.quantity + 1;
        newCart = items.filter(c => c.id !== newItem.id);
        newCart = [...newCart, newItem];
    } else {
        newCart = [...items, newItem];
    }

    window.localStorage.setItem(key, JSON.stringify(newCart));
}

export function deleteItemFromCart(id) {
    const items = cart();
    const itemExists = items.find(i => i.id === id);
        
    if (itemExists) {
        let newCart = [...items];
        itemExists.quantity = itemExists.quantity - 1;
        
        if (itemExists.quantity > 0) {
            newCart = items.filter(c => c.id !== id);
            newCart = [...newCart, itemExists];
        } else {
            newCart = items.filter(c => c.id !== id);
        }

        window.localStorage.setItem(key, JSON.stringify(newCart));
    }
}

export function clearCart() {
    window.localStorage.removeItem(key);
}