using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetExistingAsync(Guid id, CancellationToken cancellationToken);

        Task<TEntity> GetAsync(Guid id, CancellationToken cancellationToken);

        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken);

        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken);
    }
}