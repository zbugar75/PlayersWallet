using Zbugar75.PlayersWallet.Api.Domain.Entities;
using Zbugar75.PlayersWallet.Api.Infrastructure.DbContext;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.Repositories.Implementations
{
    public class WalletRepository : Repository<Wallet>, IWalletRepository
    {
        public WalletRepository(IPlayersWalletContext context) : base(context)
        {
        }
    }
}