using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zbugar75.PlayersWallet.Api.Domain.Entities;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.DbContext.Config
{
    public class TransactionResponseConfiguration : IEntityTypeConfiguration<TransactionResponse>
    {
        public void Configure(EntityTypeBuilder<TransactionResponse> builder)
        {
            builder.HasKey(item => item.TransactionId);

            builder.HasOne<Transaction>()
                .WithOne(x => x.TransactionResponse)
                .HasForeignKey<TransactionResponse>(x => x.TransactionId);

            builder
                .Property(item => item.ResponseStatusCode)
                .IsRequired();
        }
    }
}
