using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zbugar75.PlayersWallet.Api.Common.Exceptions;
using Zbugar75.PlayersWallet.Api.Infrastructure.DbContext;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.Repositories.Implementations
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly IPlayersWalletContext Context;

        public Repository(IPlayersWalletContext context)
        {
            Context = context;
        }

        public async Task<TEntity> GetExistingAsync(Guid id, CancellationToken cancellationToken)
        {
            var findEntityAsync = await Context.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken).ConfigureAwait(false);
            if (findEntityAsync == null)
                throw new EntityNotFoundException($"{typeof(TEntity)} with Id {id} not found");

            return findEntityAsync;
        }

        public async Task<TEntity> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            return await Context.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken).ConfigureAwait(false);
        }

        public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return Context.Set<TEntity>().ToListAsync(cancellationToken);
        }

        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await Context.Set<TEntity>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
            return entity;
        }
    }
}