using NSubstitute;
using PizzaItaliano.Services.Products.Application.Commands;
using PizzaItaliano.Services.Products.Application.Commands.Handlers;
using PizzaItaliano.Services.Products.Application.Services;
using PizzaItaliano.Services.Products.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Tests.Unit.Applications.Commands
{
    public class UpdateProductHandlerTests
    {
        private Task Act(UpdateProduct command) => _handler.HandleAsync(command);

        #region Arrange

        private readonly UpdateProductHandler _handler;
        private readonly IProductRepository _productRepository;
        private readonly IEventProcessor _eventProcessor;

        public UpdateProductHandlerTests()
        {
            _productRepository = Substitute.For<IProductRepository>();
            _eventProcessor = Substitute.For<IEventProcessor>();
            _handler = new UpdateProductHandler(_productRepository, _eventProcessor);
        }

        #endregion
    }
}
