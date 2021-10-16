using Convey.CQRS.Queries;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
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
    internal sealed class GetOrdersHandler : IQueryHandler<GetOrders, IEnumerable<OrderDto>>
    {
        private readonly IMongoDatabase _mongoDatabase;

        public GetOrdersHandler(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public async Task<IEnumerable<OrderDto>> HandleAsync(GetOrders query)
        {
            var collection = _mongoDatabase.GetCollection<OrderDocument>("orders");

            if (!query.OrderStatus.HasValue)
            {
                var allProductDocuments = await collection.Find(p => true).ToListAsync();
                var allDtos = allProductDocuments.Select(p => p.AsDto());
                return allDtos;
            }

            var orderDocuments = await collection.AsQueryable().Where(o => o.OrderStatus == query.OrderStatus).ToListAsync();
            var dtos = orderDocuments.Select(p => p.AsDto());
            return dtos;
        }
    }
}
