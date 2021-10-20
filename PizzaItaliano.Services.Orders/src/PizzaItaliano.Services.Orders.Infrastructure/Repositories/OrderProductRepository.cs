using Convey.Persistence.MongoDB;
using PizzaItaliano.Services.Orders.Core.Entities;
using PizzaItaliano.Services.Orders.Core.Repositories;
using PizzaItaliano.Services.Orders.Infrastructure.Mongo.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Infrastructure.Repositories
{
    internal sealed class OrderProductRepository : IOrderProductRepository
    {
        private readonly IMongoRepository<OrderProductDocument, Guid> _mongoRepository;

        public OrderProductRepository(IMongoRepository<OrderProductDocument, Guid> mongoRepository)
        {
            _mongoRepository = mongoRepository;
        }

        public Task AddAsync(OrderProduct orderProduct)
        {
            var orderProductDocument = orderProduct.AsDocument();
            var task = _mongoRepository.AddAsync(orderProductDocument);
            return task;
        }

        public Task DeleteAsync(AggregateId id)
        {
            var task = _mongoRepository.DeleteAsync(id);
            return task;
        }

        public Task<bool> ExistsAsync(AggregateId id)
        {
            var task = _mongoRepository.ExistsAsync(o => o.Id == id);
            return task;
        }

        public async Task<OrderProduct> GetAsync(AggregateId id)
        {
            var orderProductDocument = await _mongoRepository.GetAsync(id);
            return orderProductDocument?.AsEntity();
        }

        public Task UpdateAsync(OrderProduct orderProduct)
        {
            var orderProductDocument = orderProduct.AsDocument();
            var task = _mongoRepository.UpdateAsync(orderProductDocument);
            return task;
        }
    }
}
