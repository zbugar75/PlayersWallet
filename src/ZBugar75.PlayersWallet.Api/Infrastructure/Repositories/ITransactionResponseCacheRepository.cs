using System;
using System.Threading;
using System.Threading.Tasks;
using Zbugar75.PlayersWallet.Api.Domain.Entities;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.Repositories
{
    public interface ITransactionResponseCacheRepository
    {
        Task<TransactionResponse> AddTransactionResponseAsync(TransactionResponse transactionResponse, CancellationToken cancellationToken);

        Task<int> GetTransactionResponseAsync(Guid transactionId, CancellationToken cancellationToken);
    }
}