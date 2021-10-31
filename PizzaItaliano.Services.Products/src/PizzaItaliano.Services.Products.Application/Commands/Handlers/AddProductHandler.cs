using Convey.CQRS.Commands;
using PizzaItaliano.Services.Products.Application.Exceptions;
using PizzaItaliano.Services.Products.Application.Services;
using PizzaItaliano.Services.Products.Core.Entities;
using PizzaItaliano.Services.Products.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Application.Commands.Handlers
{
    public class AddProductHandler : ICommandHandler<AddProduct>
    {
        private readonly IProductRepository _productRepository;
        private readonly IEventProcessor _eventProcessor;

        public AddProductHandler(IProductRepository productRepository, IEventProcessor eventProcessor)
        {
            _productRepository = productRepository;
            _eventProcessor = eventProcessor;
        }

        public async Task HandleAsync(AddProduct command)
        {
            var exists = await _productRepository.ExistsAsync(command.ProductId);

            if (exists)
            {
                throw new ProductAlreadyExistsException(command.ProductId);
            }

            var product = Product.Create(command.ProductId, command.Name, command.Cost);
            await _productRepository.AddAsync(product);
            await _eventProcessor.ProcessAsync(product.Events);
        }
    }
}
