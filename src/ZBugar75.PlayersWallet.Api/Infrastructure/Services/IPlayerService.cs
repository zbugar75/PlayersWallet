using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbugar75.PlayersWallet.Api.Domain.Entities;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.Services
{
    public interface IPlayerService
    {
        Task<Player> CreatePlayerAsync(string username, CancellationToken cancellationToken);

        Task<Wallet> GetBalanceAsync(Guid playerId, CancellationToken cancellationToken);

        Task<IEnumerable<Player>> GetPlayersAsync(CancellationToken cancellationToken);

        Task<IEnumerable<Transaction>> GetTransactionsAsync(Guid playerId, CancellationToken cancellationToken);

        Task<TransactionResponse> RegisterTransactionAsync(Transaction transaction, CancellationToken cancellationToken);
    }
}
