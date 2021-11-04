using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using PizzaItaliano.Services.Products.Application.Events;
using PizzaItaliano.Services.Products.Application.Exceptions;
using PizzaItaliano.Services.Products.Application.Services;
using PizzaItaliano.Services.Products.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Application.Commands.Handlers
{
    public class DeleteProductHandler : ICommandHandler<DeleteProduct>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMessageBroker _messageBroker;

        public DeleteProductHandler(IProductRepository productRepository, IMessageBroker messageBroker)
        {
            _productRepository = productRepository;
            _messageBroker = messageBroker;
        }

        public async Task HandleAsync(DeleteProduct command)
        {
            var product = await _productRepository.GetAsync(command.ProductId);

            if (product is null)
            {
                throw new ProductNotFoundException(command.ProductId);
            }

            if (!product.CanBeDelete)
            {
                throw new CannotDeleteProductException(command.ProductId);
            }

            await _productRepository.DeleteAsync(command.ProductId);
            var productDeleted = new ProductDeleted(command.ProductId);
            var events = new List<IEvent>() { productDeleted };
            await _messageBroker.PublishAsync(events);
        }
    }
}
