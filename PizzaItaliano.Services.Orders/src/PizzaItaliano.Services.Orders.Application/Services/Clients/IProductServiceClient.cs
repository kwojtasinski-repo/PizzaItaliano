using PizzaItaliano.Services.Orders.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Services.Clients
{
    public interface IProductServiceClient
    {
        Task<ProductDto> GetAsync(Guid id);
    }
}
