using Convey.Persistence.MongoDB;
using MongoDB.Driver;
using PizzaItaliano.Services.Orders.Core.Entities;
using PizzaItaliano.Services.Orders.Core.Repositories;
using PizzaItaliano.Services.Orders.Infrastructure.Mongo.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Infrastructure.Repositories
{
    internal sealed class OrderRepository : IOrderRepository
    {
        private readonly IMongoRepository<OrderDocument, Guid> _mongoRepository;

        public OrderRepository(IMongoRepository<OrderDocument, Guid> mongoRepository)
        {
            _mongoRepository = mongoRepository;
        }

        public Task AddAsync(Order order)
        {
            var orderDocument = order.AsDocument();
            var task = _mongoRepository.AddAsync(orderDocument);
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

        public async Task<Order> GetAsync(AggregateId id)
        {
            var orderDocument = await _mongoRepository.GetAsync(id);
            return orderDocument?.AsEntity();
        }

        public Task UpdateAsync(Order order)
        {
            var orderDocument = order.AsDocument();
            var task = _mongoRepository.Collection.ReplaceOneAsync(o => o.Id == order.Id &&
                            o.Version < order.Version, orderDocument); // zapisywana najswiezsza wersja
            return task;
        }

        public IQueryable<Order> GetCollection(Expression<Func<Order, bool>> predicate)
        {
            var expression = predicate.Convert<Order, OrderDocument>();
            var paymentDocuments = _mongoRepository.Collection.AsQueryable().Where(expression);
            var payments = paymentDocuments.Map<OrderDocument, Order>();
            return payments;
        }
    }
}
