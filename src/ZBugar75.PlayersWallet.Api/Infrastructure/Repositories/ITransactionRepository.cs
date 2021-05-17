using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbugar75.PlayersWallet.Api.Domain.Entities;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.Repositories
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetAllForPlayerAsync(Guid playerId, CancellationToken cancellationToken);
    }
}