using Convey.CQRS.Queries;
using MongoDB.Driver;
using PizzaItaliano.Services.Orders.Application.DTO;
using PizzaItaliano.Services.Orders.Application.Queries;
using PizzaItaliano.Services.Orders.Infrastructure.Mongo.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Infrastructure.Mongo.Queries.Handlers
{
    internal sealed class GetOrderHandler : IQueryHandler<GetOrder, OrderDto>
    {
        private readonly IMongoDatabase _mongoDatabase;

        public GetOrderHandler(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public async Task<OrderDto> HandleAsync(GetOrder query)
        {
            var order = await _mongoDatabase.GetCollection<OrderDocument>("orders")
                .Find(p => p.Id == query.OrderId)
                .SingleOrDefaultAsync();

            return order?.AsDto();
        }
    }
}
