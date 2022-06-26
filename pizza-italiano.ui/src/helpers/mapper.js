export const mapToProducts = (objects) => {
    const products = [];

    for (const obj of objects) {
        const product = mapToProduct(obj);
        products.push(product);
    }

    return products;
}

export const mapToProduct = (obj) => {
    const product = {
        id: obj.id,
        name: obj.name,
        cost: Number(obj.cost).toFixed(2)
    }

    return product;
}