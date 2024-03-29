﻿using Convey.CQRS.Queries;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using PizzaItaliano.Services.Payments.Application.DTO;
using PizzaItaliano.Services.Payments.Application.Queries;
using PizzaItaliano.Services.Payments.Infrastructure.Mongo.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Infrastructure.Mongo.Queries.Handlers
{
    public class GetPaymentsHandler : IQueryHandler<GetPayments, IEnumerable<PaymentDto>>
    {
        private readonly IMongoDatabase _mongoDatabase;

        public GetPaymentsHandler(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public async Task<IEnumerable<PaymentDto>> HandleAsync(GetPayments query)
        {
            var collection = _mongoDatabase.GetCollection<PaymentDocument>("payments").AsQueryable();

            if (!query.PaymentStatus.HasValue)
            {
                var paymentDocuments = await collection.ToListAsync();
                var paymentDtos = paymentDocuments.Select(p => p.AsDto());
                return paymentDtos;
            }

            var payments = await collection.Where(p => p.PaymentStatus == query.PaymentStatus.Value).ToListAsync();
            var dtos = payments.Select(p => p.AsDto());
            return dtos;
        }
    }
}
