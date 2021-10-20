using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Application.Commands
{
    [Contract]
    public class DeleteProduct : ICommand
    {
        public Guid ProductId { get; }

        public DeleteProduct(Guid productId)
        {
            ProductId = productId;
        }
    }
}
