using PizzaItaliano.Services.Identity.Core.Entities;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Identity.Core.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetAsync(string token);
        Task AddAsync(RefreshToken refreshToken);
        Task UpdateAsync(RefreshToken refreshToken);
    }
}
