using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zbugar75.PlayersWallet.Api.Domain.Entities;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.Config
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.HasKey(item => item.PlayerId);

            builder.HasOne<Player>()
                .WithOne(x => x.Wallet)
                .HasForeignKey<Wallet>(x => x.PlayerId);

            builder
                .Property(item => item.Balance)
                .HasPrecision(12, 2)
                .IsRequired();
        }
    }
}
