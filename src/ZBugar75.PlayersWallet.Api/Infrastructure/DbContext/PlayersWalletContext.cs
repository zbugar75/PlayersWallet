using Microsoft.EntityFrameworkCore;
using Zbugar75.PlayersWallet.Api.Domain.Entities;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.DbContext
{
    public class PlayersWalletContext : Microsoft.EntityFrameworkCore.DbContext, IPlayersWalletContext
    {
        public PlayersWalletContext(DbContextOptions<PlayersWalletContext> options) : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }

        public DbSet<Wallet> Wallets { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<TransactionResponse> TransactionResponseCache { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PlayersWalletContext).Assembly);
        }
    }
}
