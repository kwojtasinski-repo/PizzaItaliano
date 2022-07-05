using PizzaItaliano.Services.Releases.Core.Entities;
using PizzaItaliano.Services.Releases.Core.Events;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Releases.Tests.Unit.Core.Entities
{
    public class CreateReleaseTests
    {
        private Release Act(AggregateId id, Guid orderId, Guid orderProductId, DateTime date, Guid userId) => Release.Create(id, orderId, orderProductId, date, userId);

        [Fact]
        public void given_valid_parameters_release_should_be_created()
        {
            // Arrange
            var id = new AggregateId();
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var date = DateTime.Now;
            var userId = Guid.NewGuid();

            // Act
            var payment = Act(id, orderId, orderProductId, date, userId);

            // Assert
            payment.Id.ShouldBe(id);
            payment.OrderId.ShouldBe(orderId);
            payment.OrderProductId.ShouldBe(orderProductId);
            payment.Date.ShouldBe(date);
            var @event = payment.Events.Single();
            @event.ShouldBeOfType<CreateRelease>();
        }
    }
}
