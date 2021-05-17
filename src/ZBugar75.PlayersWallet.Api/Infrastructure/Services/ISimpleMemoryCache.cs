using System;
using System.Threading.Tasks;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.Services
{
    public interface ISimpleMemoryCache<TItem>
    {
        Task<TItem> GetOrCreateAsync(object key, Func<Task<TItem>> createItem);
    }
}