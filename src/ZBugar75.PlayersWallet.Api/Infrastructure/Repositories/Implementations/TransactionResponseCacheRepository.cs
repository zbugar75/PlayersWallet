using Zbugar75.PlayersWallet.Api.Domain.Entities;
using Zbugar75.PlayersWallet.Api.Infrastructure.DbContext;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.Repositories.Implementations
{
    public class TransactionResponseCacheRepository : Repository<TransactionResponse>, ITransactionResponseCacheRepository
    {
        public TransactionResponseCacheRepository(IPlayersWalletContext context) : base(context)
        {
        }
    }
}