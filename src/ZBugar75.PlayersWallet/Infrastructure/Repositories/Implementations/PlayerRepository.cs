using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nito.AsyncEx;
using Zbugar75.PlayersWallet.Api.Domain.Entities;
using Zbugar75.PlayersWallet.Api.Exceptions;
using Zbugar75.PlayersWallet.Api.Infrastructure.DbContext;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.Repositories.Implementations
{
    public class PlayerRepository : Repository<Player>, IPlayerRepository
    {
        private static AsyncReaderWriterLock padlock = new AsyncReaderWriterLock();

        public PlayerRepository(IPlayersWalletContext context) : base(context)
        {
        }

        public async Task CreatePlayerAsync(Player player, CancellationToken cancellationToken)
        {
            using (await padlock.WriterLockAsync(cancellationToken).ConfigureAwait(false))
            {
                if (await Context.Players.FirstOrDefaultAsync(p => p.Username == player.Username, cancellationToken).ConfigureAwait(false) != null)
                    throw new DuplicateUsernameException($"Duplicate exception. Username '{player.Username}' already exists.");

                player.Wallet = new Wallet(0);

                await AddAsync(player, cancellationToken).ConfigureAwait(false);
                await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<decimal> GetBalanceAsync(Guid playerId, CancellationToken cancellationToken)
        {
            using (await padlock.ReaderLockAsync(cancellationToken).ConfigureAwait(false))
            {
                var player = await GetAsync(playerId, cancellationToken).ConfigureAwait(false);

                if (player == null)
                    throw new EntityNotFoundException($"Player {playerId} not found.");

                var walletBalance = player.Wallet.Balance;

                return walletBalance;
            }
        }

        public async Task<IEnumerable<Player>> GetPlayersAsync(CancellationToken cancellationToken)
        {
            using (await padlock.ReaderLockAsync(cancellationToken).ConfigureAwait(false))
            {
                return await GetAllAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}