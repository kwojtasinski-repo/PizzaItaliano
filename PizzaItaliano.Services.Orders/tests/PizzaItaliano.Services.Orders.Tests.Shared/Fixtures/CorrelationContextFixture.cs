using Newtonsoft.Json;
using System;
using static PizzaItaliano.Services.Orders.Tests.Shared.Fixtures.UserFixture;

namespace PizzaItaliano.Services.Orders.Tests.Shared.Fixtures
{
    public class CorrelationContextFixture
    {
        public static string CreateSimpleContextAndReturnAsJson(User user)
        {
            var context = new CorrelationContextFixture
            {
                User = user
            };
            return JsonConvert.SerializeObject(context);
        }

        public string CorrelationId { get; set; } = Guid.NewGuid().ToString("N");
        public string SpanContext { get; set; } = Guid.NewGuid().ToString("N");
        public User User { get; set; }
        public string ResourceId { get; set; } = Guid.NewGuid().ToString("N");
        public string TraceId { get; set; } = Guid.NewGuid().ToString("N");
        public string ConnectionId { get; set; } = Guid.NewGuid().ToString("N");
        public string Name { get; set; } = Guid.NewGuid().ToString("N");
        public DateTime CreatedAt { get; set; } = DateTime.Now.AddSeconds(-25.5);
    }
}
