using System;
using System.Threading;
using System.Threading.Tasks;
using Zbugar75.PlayersWallet.Api.Domain.Entities;
using Zbugar75.PlayersWallet.Api.Exceptions;
using Zbugar75.PlayersWallet.Api.Infrastructure.DbContext;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.Repositories.Implementations
{
    public class WalletRepository : Repository<Wallet>, IWalletRepository
    {
        public WalletRepository(IPlayersWalletContext context) : base(context)
        {
        }

        public async Task<decimal> GetBalanceAsync(Guid playerId, CancellationToken cancellationToken)
        {
            var wallet = await GetAsync(playerId, cancellationToken).ConfigureAwait(false);

            if (wallet == null)
                throw new EntityNotFoundException($"Wallet for player {playerId} not found.");

            var walletBalance = wallet.Balance;

            return walletBalance;
        }
    }
}