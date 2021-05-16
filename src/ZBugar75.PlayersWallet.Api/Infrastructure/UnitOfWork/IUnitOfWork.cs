using System;
using System.Threading;
using System.Threading.Tasks;
using Zbugar75.PlayersWallet.Api.Infrastructure.Repositories;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IPlayerRepository Players { get; }

        IWalletRepository Wallets { get; }

        ITransactionRepository Transactions { get; }

        ITransactionResponseCacheRepository TransactionResponseCache { get; }

        Task CommitAsync(CancellationToken cancellationToken);
    }
}