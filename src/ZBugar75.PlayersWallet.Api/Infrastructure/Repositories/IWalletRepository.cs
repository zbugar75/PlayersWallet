using System;
using System.Threading;
using System.Threading.Tasks;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.Repositories
{
    public interface IWalletRepository
    {
        Task<decimal> GetBalanceAsync(Guid playerId, CancellationToken cancellationToken);
    }
}