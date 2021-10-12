using PizzaItaliano.Services.Products.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Core.Repositories
{
    public interface IProductRepository
    {
        Task<Product> GetAsync(AggregateId id);
        Task<bool> ExistsAsync(AggregateId id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(AggregateId id);
    }
}
