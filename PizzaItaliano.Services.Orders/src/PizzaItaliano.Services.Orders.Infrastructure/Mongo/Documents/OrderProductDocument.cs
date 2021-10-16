using Convey.Types;
using PizzaItaliano.Services.Orders.Core.Entities;
using System;

namespace PizzaItaliano.Services.Orders.Infrastructure.Mongo.Documents
{
    internal sealed class OrderProductDocument : IIdentifiable<Guid>
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public int Cost { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public OrderProductStatus OrderProductStatus { get; set; }
    }
}