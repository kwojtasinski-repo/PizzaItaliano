using Convey.CQRS.Queries;
using PizzaItaliano.Services.Products.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Application.Queries
{
    public class GetProduct : IQuery<ProductDto>
    {
        public Guid ProductId { get; set; }
    }
}
