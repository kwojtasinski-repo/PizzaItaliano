using System;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Identity.Application.Services
{
    public interface IRefreshTokenService
    {
        Task<string> CreateAsync(Guid userId);
    }
}
