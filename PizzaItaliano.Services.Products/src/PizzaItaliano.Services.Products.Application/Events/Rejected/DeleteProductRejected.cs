using Convey.CQRS.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Application.Events.Rejected
{
    [Contract]
    public class DeleteProductRejected : IRejectedEvent
    {
        public Guid ProductId { get; }
        public string Code { get; }
        public string Reason { get; }

        public DeleteProductRejected(Guid productId, string reason, string code)
        {
            ProductId = productId;
            Reason = reason;
            Code = code;
        }
    }
}
