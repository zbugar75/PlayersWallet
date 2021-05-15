using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbugar75.PlayersWallet.Api.Domain.Entities;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.Repositories
{
    public interface IPlayerRepository
    {
        Task<Player> CreatePlayerAsync(string username, CancellationToken cancellationToken);

        Task<IEnumerable<Player>> GetPlayersAsync(CancellationToken cancellationToken);
    }
}