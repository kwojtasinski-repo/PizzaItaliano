using Newtonsoft.Json;
using PizzaItaliano.Services.Payments.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Payments.Tests.Shared;
using PizzaItaliano.Services.Payments.Tests.Shared.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Tests.EndToEnd.Helpers
{
    internal sealed class TestHelper
    {
        public static StringContent GetContent(object value)
            => new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");

        public static T MapTo<T>(string stringResponse)
        {
            var result = JsonConvert.DeserializeObject<T>(stringResponse);
            return result;
        }

        public static Task AddTestPayment(Guid paymentId, MongoDbFixture<PaymentDocument, Guid> mongoDbFixture)
        {
            var orderId = Guid.NewGuid();
            var createDate = DateTime.Now;
            var modifiedDate = DateTime.Now;
            var cost = new decimal(100);
            var status = Core.Entities.PaymentStatus.Unpaid;
            var paymentNumber = "PAY/2021/11/04/1";
            var document = new PaymentDocument { Id = paymentId, Cost = cost, CreateDate = createDate, ModifiedDate = modifiedDate, OrderId = orderId, PaymentNumber = paymentNumber, PaymentStatus = status };

            return mongoDbFixture.InsertAsync(document);
        }

        public static Task AddTestPayment(Guid paymentId, Guid orderId, Core.Entities.PaymentStatus status, MongoDbFixture<PaymentDocument, Guid> mongoDbFixture)
        {
            var createDate = DateTime.Now;
            var modifiedDate = DateTime.Now;
            var cost = new decimal(100);
            var paymentNumber = "PAY/2021/11/04/1";
            var document = new PaymentDocument { Id = paymentId, Cost = cost, CreateDate = createDate, ModifiedDate = modifiedDate, OrderId = orderId, PaymentNumber = paymentNumber, PaymentStatus = status };

            return mongoDbFixture.InsertAsync(document);
        }

        internal sealed class Error
        {
            public string Code { get; set; }
            public string Reason { get; set; }
        }
    }
}
