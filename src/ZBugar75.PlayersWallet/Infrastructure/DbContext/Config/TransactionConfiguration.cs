using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zbugar75.PlayersWallet.Api.Domain.Entities;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.DbContext.Config
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(item => item.Id);

            builder.HasOne<Player>()
                .WithOne(x => x.Transaction)
                .HasForeignKey<Transaction>(x => x.PlayerId);

            builder
                .Property(item => item.TransactionType)
                .IsRequired();

            builder
                .Property(item => item.Amount)
                .HasPrecision(12, 2)
                .IsRequired();
        }
    }
}
