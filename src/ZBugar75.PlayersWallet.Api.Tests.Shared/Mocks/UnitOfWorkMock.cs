using System;
using System.Threading;
using System.Threading.Tasks;
using Zbugar75.PlayersWallet.Api.Infrastructure.DbContext;
using Zbugar75.PlayersWallet.Api.Infrastructure.Repositories;
using Zbugar75.PlayersWallet.Api.Infrastructure.Repositories.Implementations;
using Zbugar75.PlayersWallet.Api.Infrastructure.UnitOfWork;

namespace ZBugar75.PlayersWallet.Api.Tests.Shared.Mocks
{
    public class UnitOfWorkMock : IUnitOfWork
    {
        public IPlayerRepository Players { get; set; }

        public IWalletRepository Wallets { get; set; }

        public ITransactionRepository Transactions { get; set; }

        public ITransactionResponseCacheRepository TransactionResponseCache { get; set; }

        public readonly IPlayersWalletContext Context;
        private bool _disposed = false;

        public UnitOfWorkMock(IPlayersWalletContext context)
        {
            Context = context;
            Players = new PlayerRepository(Context);
            Wallets = new WalletRepository(Context);
            Transactions = new TransactionRepository(Context);
            TransactionResponseCache = new TransactionResponseCacheRepository(Context);
        }

        public Task CommitAsync(CancellationToken cancellationToken)
        {
            return Context.SaveChangesAsync(cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                Context.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}