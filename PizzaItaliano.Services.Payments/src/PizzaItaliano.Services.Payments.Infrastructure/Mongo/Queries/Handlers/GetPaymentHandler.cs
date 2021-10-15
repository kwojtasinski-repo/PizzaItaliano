using Convey.CQRS.Queries;
using MongoDB.Driver;
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
    public class GetPaymentHandler : IQueryHandler<GetPayment, PaymentDto>
    {
        private readonly IMongoDatabase _mongoDatabase;

        public GetPaymentHandler(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public async Task<PaymentDto> HandleAsync(GetPayment query)
        {
           var payment = await _mongoDatabase.GetCollection<PaymentDocument>("payments")
                .Find(p => p.Id == query.PaymentId)
                .SingleOrDefaultAsync();

            return payment?.AsDto();
        }
    }
}
