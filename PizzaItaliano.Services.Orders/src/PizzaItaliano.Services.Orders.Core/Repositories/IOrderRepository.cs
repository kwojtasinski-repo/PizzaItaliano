using PizzaItaliano.Services.Orders.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
