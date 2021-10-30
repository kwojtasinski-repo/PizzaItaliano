using NSubstitute;
using PizzaItaliano.Services.Products.Application.Events.External;
using PizzaItaliano.Services.Products.Application.Events.External.Handlers;
using PizzaItaliano.Services.Products.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Tests.Unit.Applications.Events
{
    public class OrderProductAddedHandlerTests
    {
        private Task Act(OrderProductAdded command) => _handler.HandleAsync(command);

        #region Arrange

        private readonly OrderProductAddedHandler _handler;
        private readonly IProductRepository _productRepository;

        public OrderProductAddedHandlerTests()
        {
            _productRepository = Substitute.For<IProductRepository>();
            _handler = new OrderProductAddedHandler(_productRepository);
        }

        #endregion
    }
}
