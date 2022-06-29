using Convey.CQRS.Queries;
using MongoDB.Driver;
using PizzaItaliano.Services.Payments.Application.DTO;
using PizzaItaliano.Services.Payments.Application.Queries;
using PizzaItaliano.Services.Payments.Infrastructure.Mongo.Documents;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Infrastructure.Mongo.Queries.Handlers
{
    public class GetPaymentByOrderIdHandler : IQueryHandler<GetPaymentByOrderId, PaymentDto>
    {
        private readonly IMongoDatabase _mongoDatabase;

        public GetPaymentByOrderIdHandler(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public async Task<PaymentDto> HandleAsync(GetPaymentByOrderId query)
        {
            var payment = await _mongoDatabase.GetCollection<PaymentDocument>("payments")
                 .Find(p => p.OrderId == query.OrderId)
                 .SingleOrDefaultAsync();

            return payment?.AsDto();
        }
    }
}
