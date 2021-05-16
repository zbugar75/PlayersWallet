using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zbugar75.PlayersWallet.Api.Domain.Entities;
using Zbugar75.PlayersWallet.Api.Infrastructure.DbContext;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.Repositories.Implementations
{
    public class PlayerRepository : Repository<Player>, IPlayerRepository
    {
        public PlayerRepository(IPlayersWalletContext context) : base(context)
        {
        }

        public async Task<bool> ExistsPlayerWithUsernameAsync(string username, CancellationToken cancellationToken)
        {
            return await Context.Players.FirstOrDefaultAsync(p => p.Username == username, cancellationToken).ConfigureAwait(false) != null;
        }
    }
}