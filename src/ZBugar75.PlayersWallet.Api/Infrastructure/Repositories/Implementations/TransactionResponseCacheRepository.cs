using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nito.AsyncEx;
using Zbugar75.PlayersWallet.Api.Domain.Entities;
using Zbugar75.PlayersWallet.Api.Exceptions;
using Zbugar75.PlayersWallet.Api.Infrastructure.DbContext;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.Repositories.Implementations
{
    public class TransactionResponseCacheRepository : Repository<TransactionResponse>, ITransactionResponseCacheRepository
    {
        private static AsyncReaderWriterLock padlock = new AsyncReaderWriterLock();

        public TransactionResponseCacheRepository(IPlayersWalletContext context) : base(context)
        {
        }

        public async Task<TransactionResponse> AddTransactionResponseAsync(TransactionResponse transactionResponse, CancellationToken cancellationToken)
        {
            using (await padlock.WriterLockAsync(cancellationToken).ConfigureAwait(false))
            {
                if (await Context.TransactionResponseCache.FirstOrDefaultAsync(p => p.TransactionId == transactionResponse.TransactionId, cancellationToken).ConfigureAwait(false) != null)
                    throw new DuplicateUsernameException($"Duplicate exception. Transaction '{transactionResponse.TransactionId}' already exists.");

                await AddAsync(transactionResponse, cancellationToken).ConfigureAwait(false);
                await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return transactionResponse;
            }
        }

        public async Task<int> GetTransactionResponseAsync(Guid transactionId, CancellationToken cancellationToken)
        {
            return (await GetAsync(transactionId, cancellationToken).ConfigureAwait(false)).ResponseStatusCode;
        }
    }
}