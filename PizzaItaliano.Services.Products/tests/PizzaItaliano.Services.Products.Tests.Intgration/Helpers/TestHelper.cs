using Newtonsoft.Json;
using PizzaItaliano.Services.Products.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Products.Tests.Shared.Fixtures;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Tests.Intgration.Helpers
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

        public static Task AddTestProduct(Guid productId, MongoDbFixture<ProductDocument, Guid> mongoDbFixture)
        {
            var name = "product1";
            var cost = new decimal(100);
            var status = Core.Entities.ProductStatus.New;
            var document = new ProductDocument { Id=productId, Cost=cost, Name=name, Status = status };

            return mongoDbFixture.InsertAsync(document);
        }

        public static Task AddTestProduct(Guid productId, Core.Entities.ProductStatus status, MongoDbFixture<ProductDocument, Guid> mongoDbFixture)
        {
            var name = "product1";
            var cost = new decimal(100);
            var document = new ProductDocument { Id = productId, Cost = cost, Name = name, Status = status };

            return mongoDbFixture.InsertAsync(document);
        }

        internal sealed class Error
        {
            public string Code { get; set; }
            public string Reason { get; set; }
        }
    }
}
