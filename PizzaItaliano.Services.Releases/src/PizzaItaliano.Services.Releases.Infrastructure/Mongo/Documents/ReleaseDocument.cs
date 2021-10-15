using Convey.Types;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Releases.Infrastructure.Mongo.Documents
{
    internal sealed class ReleaseDocument : IIdentifiable<Guid>
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid OrderProductId { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; }
    }
}
