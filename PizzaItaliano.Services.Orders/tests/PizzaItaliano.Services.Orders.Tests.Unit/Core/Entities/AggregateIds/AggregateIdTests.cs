using PizzaItaliano.Services.Orders.Core.Entities;
using PizzaItaliano.Services.Orders.Core.Exceptions;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Orders.Tests.Unit.Core.Entities.AggregateIds
{
    public class AggregateIdTests
    {
        private AggregateId Act(Guid id) => new AggregateId(id);

        [Fact]
        public void given_valid_guid_should_create_aggregate_id()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var aggregateId = Act(id);

            // Assert
            aggregateId.Value.ShouldNotBe(Guid.Empty);
            aggregateId.Value.ShouldBe(id);
        }

        [Fact]
        public void given_invalid_guid_should_throw_an_exception()
        {
            // Arrange
            var id = Guid.Empty;

            // Act
            var exception = Record.Exception(() => Act(id));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<InvalidAggregateIdException>();
        }
    }
}
