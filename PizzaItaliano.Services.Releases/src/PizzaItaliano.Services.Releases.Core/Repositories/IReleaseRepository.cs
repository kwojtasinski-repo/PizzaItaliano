using PizzaItaliano.Services.Releases.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Releases.Core.Repositories
{
    public interface IReleaseRepository
    {
        Task<Release> GetAsync(AggregateId id);
        Task<bool> ExistsAsync(AggregateId id);
        Task AddAsync(Release release);
        Task UpdateAsync(Release release);
        Task DeleteAsync(AggregateId id);
    }
}
