using System;
using System.Threading;
using System.Threading.Tasks;
using Zbugar75.PlayersWallet.Api.Infrastructure.DbContext;
using Zbugar75.PlayersWallet.Api.Infrastructure.Repositories;
using Zbugar75.PlayersWallet.Api.Infrastructure.Repositories.Implementations;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IPlayersWalletContext _context;
        private bool _disposed = false;

        public UnitOfWork(IPlayersWalletContext context)
        {
            _context = context;
            Players = new PlayerRepository(_context);
            Wallets = new WalletRepository(_context);
            //Transactions = new TransactionRepository(_context);
        }

        public IPlayerRepository Players { get; private set; }
        public IWalletRepository Wallets { get; private set; }
        public ITransactionRepository Transactions { get; private set; }


        public Task CommitAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
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