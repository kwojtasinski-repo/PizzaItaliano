using Convey.Types;
using PizzaItaliano.Services.Products.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Infrastructure.Mongo.Documents
{
    internal sealed class ProductDocument : IIdentifiable<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public ProductStatus Status { get; set; }
    }
}
