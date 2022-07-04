using Convey.CQRS.Queries;
using MongoDB.Driver;
using PizzaItaliano.Services.Orders.Application;
using PizzaItaliano.Services.Orders.Application.DTO;
using PizzaItaliano.Services.Orders.Application.Queries;
using PizzaItaliano.Services.Orders.Infrastructure.Mongo.Documents;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Infrastructure.Mongo.Queries.Handlers
{
    internal sealed class GetOrdersByUserIdHandler : IQueryHandler<GetOrdersByUserId, IEnumerable<OrderDto>>
    {
        private readonly IMongoDatabase _mongoDatabase;

        public GetOrdersByUserIdHandler(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public Task<IEnumerable<OrderDto>> HandleAsync(GetOrdersByUserId query)
        {
            var collection = _mongoDatabase.GetCollection<OrderDocument>("orders");
            var orderDocuments = collection.AsQueryable().Where(o => o.Email == query.UserId).ToList();
            var dtos = orderDocuments.Select(p => p.AsDto());
            return Task.FromResult(dtos);
        }
    }
}
