using Convey.CQRS.Commands;
using PizzaItaliano.Services.Products.Application.Exceptions;
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

        public DeleteProductHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
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
        }
    }
}
