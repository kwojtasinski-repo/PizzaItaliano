using NSubstitute;
using PizzaItaliano.Services.Products.Application.Commands;
using PizzaItaliano.Services.Products.Application.Commands.Handlers;
using PizzaItaliano.Services.Products.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Tests.Unit.Applications.Commands
{
    public class DeleteProductHandlerTests
    {
        private Task Act(DeleteProduct command) => _handler.HandleAsync(command);

        #region Arrange

        private readonly DeleteProductHandler _handler;
        private readonly IProductRepository _productRepository;

        public DeleteProductHandlerTests()
        {
            _productRepository = Substitute.For<IProductRepository>();
            _handler = new DeleteProductHandler(_productRepository);
        }

        #endregion
    }
}
