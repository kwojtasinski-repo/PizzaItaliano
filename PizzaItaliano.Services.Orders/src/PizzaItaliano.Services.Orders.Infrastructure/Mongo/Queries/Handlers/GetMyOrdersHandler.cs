using Convey.CQRS.Queries;
using MongoDB.Driver;
using PizzaItaliano.Services.Orders.Application;
using PizzaItaliano.Services.Orders.Application.DTO;
using PizzaItaliano.Services.Orders.Application.Queries;
using PizzaItaliano.Services.Orders.Infrastructure.Mongo.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Infrastructure.Mongo.Queries.Handlers
{
    internal sealed class GetMyOrdersHandler : IQueryHandler<GetMyOrders, IEnumerable<OrderDto>>
    {
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IAppContext _appContext;

        public GetMyOrdersHandler(IMongoDatabase mongoDatabase, IAppContext appContext)
        {
            _mongoDatabase = mongoDatabase;
            _appContext = appContext;
        }

        public Task<IEnumerable<OrderDto>> HandleAsync(GetMyOrders query)
        {
            if (_appContext.Identity is null)
            {
                return Task.FromResult<IEnumerable<OrderDto>>(Array.Empty<OrderDto>());
            }

            var collection = _mongoDatabase.GetCollection<OrderDocument>("orders");
            var orderDocuments = collection.AsQueryable().Where(o => o.UserId == _appContext.Identity.Id).ToList();
            var dtos = orderDocuments.Select(p => p.AsDto());
            return Task.FromResult(dtos);
        }
    }
}
