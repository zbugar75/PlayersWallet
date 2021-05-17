using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Zbugar75.PlayersWallet.Api.Domain.Entities;
using Zbugar75.PlayersWallet.Api.Infrastructure.DbContext;

namespace ZBugar75.PlayersWallet.Api.Tests.Shared.Builder
{
    public class TransactionResponseBuilder
    {
        private Guid _transactionId;
        private int _responseStatusCode;

        public TransactionResponseBuilder()
        {
            _transactionId = Guid.NewGuid();
            _responseStatusCode = StatusCodes.Status200OK;
        }

        public TransactionResponseBuilder WithTransactionId(Guid transactionId)
        {
            _transactionId = transactionId;
            return this;
        }

        public TransactionResponseBuilder WithResponseStatusCode(int responseStatusCode)
        {
            _responseStatusCode = responseStatusCode;
            return this;
        }

        public TransactionResponse Build()
        {
            return new TransactionResponse
            {
                TransactionId = _transactionId,
                ResponseStatusCode = _responseStatusCode
            };
        }

        public async Task<TransactionResponse> CreateAsync(IPlayersWalletContext dbContext, CancellationToken cancellationToken)
        {
            var entity = Build();

            await dbContext.TransactionResponseCache.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return entity;
        }
    }
}
