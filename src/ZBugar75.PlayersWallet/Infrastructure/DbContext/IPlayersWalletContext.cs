using Microsoft.EntityFrameworkCore;
using Zbugar75.PlayersWallet.Api.Domain.Entities;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.DbContext
{
    public interface IPlayersWalletContext
    {
        DbSet<Player> Players { get; }

        DbSet<Wallet> Wallets { get; }

        DbSet<Transaction> Transactions { get; }

        DbSet<TransactionResponseCache> TransactionResponseCaches { get; }
    }
}