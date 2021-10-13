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
    public class UpdateProductHandler : ICommandHandler<UpdateProduct>
    {
        private readonly IProductRepository _productRepository;

        public UpdateProductHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task HandleAsync(UpdateProduct command)
        {
            bool emptyName = string.IsNullOrEmpty(command.Name);
            bool costHasValue = command.Cost.HasValue;

            if (emptyName && !costHasValue)
            {
                return;
            }

            if (costHasValue && command.Cost < 0)
            {
                throw new InvalidProductCostException(command.ProductId);
            }

            var product = await _productRepository.GetAsync(command.ProductId);

            if (product is null)
            {
                throw new ProductNotFoundException(command.ProductId);
            }

            if (costHasValue)
            {
                if (emptyName)
                {
                    product.Modified(command.Cost.Value);
                }
                else
                {
                    product.Modified(command.Name, command.Cost.Value);
                }
            }
            else
            {
                product.Modified(command.Name);
            }

            await _productRepository.UpdateAsync(product);
        }
    }
}
