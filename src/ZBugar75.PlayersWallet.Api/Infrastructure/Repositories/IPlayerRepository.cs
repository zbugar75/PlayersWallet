using System.Threading;
using System.Threading.Tasks;
using Zbugar75.PlayersWallet.Api.Domain.Entities;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.Repositories
{
    public interface IPlayerRepository : IRepository<Player>
    {
        Task<bool> ExistsPlayerWithUsernameAsync(string username, CancellationToken cancellationToken);
    }
}