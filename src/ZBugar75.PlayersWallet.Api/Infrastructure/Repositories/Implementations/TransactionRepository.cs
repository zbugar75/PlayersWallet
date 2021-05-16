using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zbugar75.PlayersWallet.Api.Domain.Entities;
using Zbugar75.PlayersWallet.Api.Infrastructure.DbContext;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.Repositories.Implementations
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(IPlayersWalletContext context) : base(context)
        {
        }

        //public async Task<Transaction> RegisterTransactionAsync(Transaction transaction, CancellationToken cancellationToken)
        //{
        //    using (await padlock.WriterLockAsync(cancellationToken).ConfigureAwait(false))
        //    {
        //        await AddAsync(transaction, cancellationToken).ConfigureAwait(false);

        //        return transaction;
        //    }
        //}

        public async Task<IEnumerable<Transaction>> GetAllForPlayerAsync(Guid playerId, CancellationToken cancellationToken)
        {
            return await Context.Transactions
                .Where(t => t.PlayerId == playerId).ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}