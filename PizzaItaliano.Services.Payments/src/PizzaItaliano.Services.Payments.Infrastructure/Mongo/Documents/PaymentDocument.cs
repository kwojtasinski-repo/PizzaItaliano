using Convey.Types;
using MongoDB.Bson.Serialization.Attributes;
using PizzaItaliano.Services.Payments.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Infrastructure.Mongo.Documents
{
    internal sealed class PaymentDocument : IIdentifiable<Guid>
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public decimal Cost { get; set; }
        public Guid OrderId { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateDate { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ModifiedDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }
}
