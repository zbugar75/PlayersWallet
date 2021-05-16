using System;
using System.Threading;
using System.Threading.Tasks;
using Zbugar75.PlayersWallet.Api.Domain.Entities;
using Zbugar75.PlayersWallet.Api.Domain.Entities.Enums;
using Zbugar75.PlayersWallet.Api.Infrastructure.DbContext;
using ZBugar75.PlayersWallet.Api.Tests.Shared.Extensions;

namespace ZBugar75.PlayersWallet.Api.Tests.Shared.Builder
{
    public class TransactionBuilder
    {
        private Guid _id;
        private TransactionType _transactionType;
        private decimal _amount;
        private Guid _palyerId;

        public TransactionBuilder()
        {
            _id = Guid.NewGuid();
            _transactionType = TransactionType.Win;
            _amount = 0;
            _palyerId = 1.ToGuid();
        }

        public TransactionBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public TransactionBuilder WithTransactionType(TransactionType transactionType)
        {
            _transactionType = transactionType;
            return this;
        }

        public TransactionBuilder WithAmount(decimal amount)
        {
            _amount = amount;
            return this;
        }

        public TransactionBuilder WithPalyerId(Guid palyerId)
        {
            _palyerId = palyerId;
            return this;
        }

        public Transaction Build()
        {
            return new Transaction
            {
                Id = _id,
                TransactionType = _transactionType,
                Amount = _amount,
                PlayerId = _palyerId
            };
        }

        public async Task<Transaction> CreateAsync(IPlayersWalletContext dbContext, CancellationToken cancellationToken)
        {
            var entity = Build();

            await dbContext.Transactions.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return entity;
        }
    }
}
