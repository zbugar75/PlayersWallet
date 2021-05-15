using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zbugar75.PlayersWallet.Api.Domain.Entities;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.DbContext.Config
{
    public class TransactionResponseCacheConfiguration : IEntityTypeConfiguration<TransactionResponseCache>
    {
        public void Configure(EntityTypeBuilder<TransactionResponseCache> builder)
        {
            builder.HasKey(item => item.TransactionId);

            builder.HasOne<Transaction>()
                .WithOne(x => x.TransactionResponseCache)
                .HasForeignKey<TransactionResponseCache>(x => x.TransactionId);

            builder
                .Property(item => item.ResponseStatusCode)
                .IsRequired();
        }
    }
}
