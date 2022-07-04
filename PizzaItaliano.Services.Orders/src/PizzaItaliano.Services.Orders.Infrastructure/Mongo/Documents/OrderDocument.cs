using Convey.Types;
using MongoDB.Bson.Serialization.Attributes;
using PizzaItaliano.Services.Orders.Core.Entities;
using System;
using System.Collections.Generic;

namespace PizzaItaliano.Services.Orders.Infrastructure.Mongo.Documents
{
    internal sealed class OrderDocument : IIdentifiable<Guid>
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public decimal Cost { get; set; }
        public OrderStatus OrderStatus { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime OrderDate { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? ReleaseDate { get; set; }
        public Guid UserId { get; set; }
        public IEnumerable<OrderProductDocument> OrderProductDocuments { get; set; }
        public int Version { get; set; }
    }
}
