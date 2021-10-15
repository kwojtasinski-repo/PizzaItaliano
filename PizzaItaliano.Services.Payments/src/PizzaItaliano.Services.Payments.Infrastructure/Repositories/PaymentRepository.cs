using Convey.Persistence.MongoDB;
using MongoDB.Driver;
using PizzaItaliano.Services.Payments.Core.Entities;
using PizzaItaliano.Services.Payments.Core.Repositories;
using PizzaItaliano.Services.Payments.Infrastructure.Mongo.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Infrastructure.Repositories
{
    internal sealed class PaymentRepository : IPaymentRepository
    {
        private readonly IMongoRepository<PaymentDocument, Guid> _mongoRepository;

        public PaymentRepository(IMongoRepository<PaymentDocument, Guid> mongoRepository)
        {
            _mongoRepository = mongoRepository;
        }

        public Task AddAsync(Payment payment)
        {
            var paymentDocument = payment.AsDocument();
            var task = _mongoRepository.AddAsync(paymentDocument);
            return task;
        }

        public Task DeleteAsync(AggregateId id)
        {
            var task = _mongoRepository.DeleteAsync(id);
            return task;
        }

        public Task<bool> ExistsAsync(AggregateId id)
        {
            var exists = _mongoRepository.ExistsAsync(p => p.Id == id);
            return exists;
        }

        public async Task<Payment> GetAsync(AggregateId id)
        {
            var paymentDocument = await _mongoRepository.GetAsync(id);
            return paymentDocument?.AsEntity();
        }

        public Task UpdateAsync(Payment payment)
        {
            var paymentDocument = payment.AsDocument();
            var task = _mongoRepository.UpdateAsync(paymentDocument);
            return task;
        }

        public IQueryable<Payment> GetCollection(Expression<Func<Payment, bool>> predicate)
        {
            var expression = predicate.Convert<Payment, PaymentDocument>();
            var paymentDocuments = _mongoRepository.Collection.AsQueryable().Where(expression);
            var payments = paymentDocuments.Map<PaymentDocument, Payment>();
            return payments;
        }
    }
}
