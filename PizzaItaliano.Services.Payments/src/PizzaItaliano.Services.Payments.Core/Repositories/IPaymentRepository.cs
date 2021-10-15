using PizzaItaliano.Services.Payments.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Core.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment> GetAsync(AggregateId id);
        Task<bool> ExistsAsync(AggregateId id);
        Task AddAsync(Payment payment);
        Task UpdateAsync(Payment payment);
        Task DeleteAsync(AggregateId id);
        IQueryable<Payment> GetCollection(Expression<Func<Payment, bool>> predicate);
    }
}
