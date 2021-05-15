using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbugar75.PlayersWallet.Api.Domain.Entities;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.Repositories
{
    public interface IPlayerRepository
    {
        Task CreatePlayerAsync(Player player, CancellationToken cancellationToken);

        Task<decimal> GetBalanceAsync(Guid playerId, CancellationToken cancellationToken);

        Task<IEnumerable<Player>> GetPlayersAsync(CancellationToken cancellationToken);
    }
}