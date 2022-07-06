using PizzaItaliano.Services.Releases.Application.Commands;
using PizzaItaliano.Services.Releases.Application.Events;
using PizzaItaliano.Services.Releases.Application.Events.Rejected;
using PizzaItaliano.Services.Releases.Application.Exceptions;
using PizzaItaliano.Services.Releases.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Releases.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Releases.Tests.Intgration
{
    [Collection("Collection")]
    public class AddReleaseTests
    {
        private Task Act(AddRelease command) => _rabbitMqFixture.PublishAsync(command, Exchange);

        [Fact]
        public async Task add_release_command_should_add_document_with_given_id_to_database()
        {
            var releaseId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var command = new AddRelease() { ReleaseId = releaseId, OrderId = orderId, OrderProductId = orderProductId };

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<ReleaseAdded, ReleaseDocument>(Exchange,
                    _mongoDbFixture.GetAsync, command.ReleaseId);

            await Act(command);

            var document = await tcs.Task;

            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.ReleaseId);
            document.OrderId.ShouldBe(command.OrderId);
            document.OrderProductId.ShouldBe(command.OrderProductId);
        }

        [Fact]
        public async Task add_release_command_with_invalid_id_should_throw_an_exception_and_send_rejected_event()
        {
            var releaseId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            await _mongoDbFixture.InsertAsync(new ReleaseDocument() { Id = releaseId, OrderId = orderId, OrderProductId = orderProductId, Date = DateTime.Now });
            var command = new AddRelease() { ReleaseId = releaseId, OrderId = orderId, OrderProductId = orderProductId };

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<AddReleaseRejected>(Exchange);

            await Act(command);

            var addReleaseRejected = await tcs.Task;

            addReleaseRejected.ShouldNotBeNull();
            addReleaseRejected.ShouldBeOfType<AddReleaseRejected>();
            var exception = new ReleaseAlreadyExistsException(releaseId);
            addReleaseRejected.Code.ShouldBe(exception.Code);
            addReleaseRejected.Reason.ShouldBe(exception.Message);
        }

        #region Arrange

        private const string Exchange = "release";
        private readonly MongoDbFixture<ReleaseDocument, Guid> _mongoDbFixture;
        private readonly RabbitMqFixture _rabbitMqFixture;

        public AddReleaseTests(TestAppFactory factory)
        {
            _rabbitMqFixture = new RabbitMqFixture();
            _mongoDbFixture = new MongoDbFixture<ReleaseDocument, Guid>("releases");
            factory.Server.AllowSynchronousIO = true;
        }

        #endregion
    }
}
