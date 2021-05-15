using Microsoft.EntityFrameworkCore;
using Zbugar75.PlayersWallet.Api.Domain.Entities;

namespace Zbugar75.PlayersWallet.Api.Infrastructure
{
    public class PlayersWalletContext : DbContext, IPlayersWalletContext
    {
        public PlayersWalletContext(DbContextOptions<PlayersWalletContext> options) : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }

        public DbSet<Wallet> Wallets { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<TransactionResponseCache> TransactionResponseCaches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PlayersWalletContext).Assembly);
        }
    }
}
