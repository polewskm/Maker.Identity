using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Maker.Identity.Contracts.Repositories;
using Maker.Identity.Contracts.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Data.Repositories
{
    public interface IRepositoryContext<out TContext> : IRepository
        where TContext : class
    {
        TContext Context { get; }
    }

    public class RepositoryEntityFramework<TContext, TEntity> : RepositoryBase<TEntity>, IRepositoryContext<TContext>
        where TContext : DbContext
        where TEntity : class
    {
        public TContext Context { get; }

        protected IQueryBuilder QueryBuilder { get; }

        protected DbSet<TEntity> EntitySet => Context.Set<TEntity>();

        public RepositoryEntityFramework(TContext context, IQueryBuilder queryBuilder)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            QueryBuilder = queryBuilder ?? throw new ArgumentNullException(nameof(queryBuilder));
        }

        public override async Task<TEntity> FindAsync(IQueryConfiguration<TEntity> queryConfiguration, CancellationToken cancellationToken = default)
        {
            var queryPipe = QueryBuilder.Build(queryConfiguration);
            var query = queryPipe.Query(EntitySet);

            var result = await query.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }

        public override async Task<TOut> FindAsync<TOut>(IQueryConfiguration<TEntity, TOut> queryConfiguration, CancellationToken cancellationToken = default)
        {
            var queryPipe = QueryBuilder.Build(queryConfiguration);
            var query = queryPipe.Query(EntitySet);

            var result = await query.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }

        public override async Task<IReadOnlyList<TEntity>> ListAsync(IQueryConfiguration<TEntity> queryConfiguration, CancellationToken cancellationToken = default)
        {
            var queryPipe = QueryBuilder.Build(queryConfiguration);
            var query = queryPipe.Query(EntitySet);

            var results = await query.ToListAsync(cancellationToken).ConfigureAwait(false);
            return results;
        }

        public override async Task<IReadOnlyList<TOut>> ListAsync<TOut>(IQueryConfiguration<TEntity, TOut> queryConfiguration, CancellationToken cancellationToken = default)
        {
            var queryPipe = QueryBuilder.Build(queryConfiguration);
            var query = queryPipe.Query(EntitySet);

            var results = await query.ToListAsync(cancellationToken).ConfigureAwait(false);
            return results;
        }

        public override async Task<int> CountAsync(IQueryConfiguration<TEntity> queryConfiguration, CancellationToken cancellationToken = default)
        {
            var queryPipe = QueryBuilder.Build(queryConfiguration);
            var query = queryPipe.Query(EntitySet);

            var count = await query.CountAsync(cancellationToken).ConfigureAwait(false);
            return count;
        }

        protected override async Task CoreAddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await EntitySet.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected override Task CoreUpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!EntitySet.Local.Contains(entity))
                EntitySet.Attach(entity);

            EntitySet.Update(entity);

            return Task.CompletedTask;
        }

        protected override Task CoreRemoveAsync(TEntity entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            EntitySet.Remove(entity);

            return Task.CompletedTask;
        }

    }
}