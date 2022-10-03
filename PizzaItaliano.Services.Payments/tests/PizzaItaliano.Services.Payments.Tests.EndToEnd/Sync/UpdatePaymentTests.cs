using PizzaItaliano.Services.Payments.API;
using PizzaItaliano.Services.Payments.Application.Commands;
using PizzaItaliano.Services.Payments.Core.Entities;
using PizzaItaliano.Services.Payments.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Payments.Tests.EndToEnd.Helpers;
using PizzaItaliano.Services.Payments.Tests.Shared;
using PizzaItaliano.Services.Payments.Tests.Shared.Factories;
using Shouldly;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Payments.Tests.EndToEnd.Sync
{
    [Collection("Collection")]
    public class UpdatePaymentTests
    {
        private Task<HttpResponseMessage> Act(UpdatePayment command)
           => _httpClient.PutAsync($"payments/{command.PaymentId}", TestHelper.GetContent(command));

        [Fact]
        public async Task should_update_payment_and_change_status_to_paid()
        {
            var paymentId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            await TestHelper.AddTestPayment(paymentId, orderId, PaymentStatus.Unpaid, _mongoDbFixture);
            var command = new UpdatePayment { PaymentId = paymentId, PaymentStatus = PaymentStatus.Paid };

            var response = await Act(command);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var paymentUpdated = await _mongoDbFixture.GetAsync(paymentId);
            paymentUpdated.ShouldNotBeNull();
            paymentUpdated.PaymentStatus.ShouldBe(PaymentStatus.Paid);
        }

        [Fact]
        public async Task should_update_payment_and_change_status_to_unpaid()
        {
            var paymentId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            await TestHelper.AddTestPayment(paymentId, orderId, PaymentStatus.Paid, _mongoDbFixture);
            var command = new UpdatePayment { PaymentId = paymentId, PaymentStatus = PaymentStatus.Unpaid };

            var response = await Act(command);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var paymentUpdated =  await _mongoDbFixture.GetAsync(paymentId);
            paymentUpdated.ShouldNotBeNull();
            paymentUpdated.PaymentStatus.ShouldBe(PaymentStatus.Unpaid);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<PaymentDocument, Guid> _mongoDbFixture;

        public UpdatePaymentTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<PaymentDocument, Guid>("payments");
            factory.Server.AllowSynchronousIO = true;
            _httpClient = factory.CreateClient();
        }

        #endregion
    }
}
