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

export const mapToOrders = (objects) => {
    const orders = [];

    for (const obj of objects) {
        const order = mapToOrder(obj);
        orders.push(order);
    }

    return orders;
}

export const mapToOrder = (obj) => {
    const order = {
        id: obj.id,
        orderNumber: obj.orderNumber,
        orderStatus: obj.orderStatus,
        orderDate: new Date(obj.orderDate).toLocaleString(),
        cost: Number(obj.cost).toFixed(2),
        releaseDate: obj.releaseDate ? new Date(obj.releaseDate).toLocaleString() : null,
        orderProducts: obj.orderProducts ? mapToOrderProducts(obj.orderProducts) : [],
        userId: obj.userId
    };

    return order;
}

export const mapToOrderProducts = (objects) => {
    const orderProducts = [];

    for (const obj of objects) {
        const orderProduct = mapToOrderProduct(obj);
        orderProducts.push(orderProduct);
    }

    return orderProducts;
}

export const mapToOrderProduct = (obj) => {
    const orderProduct = {
        id: obj.id,
        productName: obj.productName,
        quantity: obj.quantity,
        cost: Number(obj.cost).toFixed(2),
        orderProductStatus: obj.orderProductStatus,
        orderId: obj.orderId,
        productId: obj.productId
    }

    return orderProduct;
}

export const mapToPayment = (obj) => {
    const payment = {
        id: obj.id,
        paymentNumber: obj.paymentNumber,
        orderId: obj.orderId,
        cost: Number(obj.cost).toFixed(2),
        paymentStatus: obj.paymentStatus,
        createDate: new Date(obj.createDate).toLocaleString(),
        modifiedDate: new Date(obj.modifiedDate).toLocaleString(),
        paid: obj.paid,
        userId: obj.userId
    }

    return payment;
}

export const mapToPayments = (objects) => {
    const payments = [];

    for (const obj of objects) {
        const payment = mapToPayment(obj);
        payments.push(payment);
    }

    return payments;
}

export const mapToRelease = (obj) => {
    const release = {
        id: obj.id,
        orderId: obj.orderId,
        orderProductId: obj.orderProductId,
        date: new Date(obj.date).toLocaleString(),
        userId: obj.userId
    }

    return release;
}

export const mapToReleases = (objects) => {
    const releases = [];

    for (const obj of objects) {
        const release = mapToRelease(obj);
        releases.push(release);
    }

    return releases;
}