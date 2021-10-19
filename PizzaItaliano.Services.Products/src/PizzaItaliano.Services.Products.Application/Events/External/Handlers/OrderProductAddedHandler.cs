using Convey.CQRS.Events;
using PizzaItaliano.Services.Products.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Application.Events.External.Handlers
{
    public class OrderProductAddedHandler : IEventHandler<OrderProductAdded>
    {
        private readonly IProductRepository _productRepository;

        public OrderProductAddedHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task HandleAsync(OrderProductAdded @event)
        {
            var product = await _productRepository.GetAsync(@event.ProductId);
            if (product is null)
            {
                return;
            }

            if (product.Status != Core.Entities.ProductStatus.New)
            {
                return;
            }

            product.MarkAsUsed();
            await _productRepository.UpdateAsync(product);
        }
    }
}
