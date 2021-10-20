using PizzaItaliano.Services.Orders.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Repositories
{
    public interface IOrderProductRepository
    {
        Task<OrderProduct> GetAsync(AggregateId id);
        Task<bool> ExistsAsync(AggregateId id);
        Task AddAsync(OrderProduct orderProduct);
        Task UpdateAsync(OrderProduct orderProduct);
        Task DeleteAsync(AggregateId id);
    }
}
