using PizzaItaliano.Services.Payments.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.Services.Clients
{
    public interface IOrderServiceClient
    {
        Task<OrderDto> GetAsync(Guid orderId);
    }
}
