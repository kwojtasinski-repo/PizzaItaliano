using PizzaItaliano.Services.Orders.Core.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> GetAsync(AggregateId id);
        Task<bool> ExistsAsync(AggregateId id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(AggregateId id);
        IQueryable<Order> GetCollection(Expression<Func<Order, bool>> predicate);
    }
}
