using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zbugar75.PlayersWallet.Api.Domain.Entities;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.DbContext
{
    public interface IPlayersWalletContext : IDisposable
    {
        DbSet<Player> Players { get; }

        DbSet<Wallet> Wallets { get; }

        DbSet<Transaction> Transactions { get; }

        DbSet<TransactionResponse> TransactionResponseCaches { get; }

        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}