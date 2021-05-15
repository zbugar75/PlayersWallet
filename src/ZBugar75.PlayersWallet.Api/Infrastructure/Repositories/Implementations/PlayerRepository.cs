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

        public async Task<Player> CreatePlayerAsync(string username, CancellationToken cancellationToken)
        {
            if (username is null)
                throw new ArgumentNullException("username");

            using (await padlock.WriterLockAsync(cancellationToken).ConfigureAwait(false))
            {
                if (await Context.Players.FirstOrDefaultAsync(p => p.Username == username, cancellationToken).ConfigureAwait(false) != null)
                    throw new DuplicateUsernameException($"Duplicate exception. Username '{username}' already exists.");

                var player = new Player
                {
                    Username = username,
                    Wallet = new Wallet(0)
                };

                await AddAsync(player, cancellationToken).ConfigureAwait(false);
                await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return player;
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