using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Maker.Identity.Contracts.Repositories;
using Maker.Identity.Contracts.Specifications;
using Maker.Identity.Data.Services;
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

        protected ISpecificationQueryBuilder SpecificationQueryBuilder { get; }

        protected DbSet<TEntity> EntitySet => Context.Set<TEntity>();

        public RepositoryEntityFramework(TContext context, ISpecificationQueryBuilder specificationQueryBuilder)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            SpecificationQueryBuilder = specificationQueryBuilder ?? throw new ArgumentNullException(nameof(specificationQueryBuilder));
        }

        public override async Task<TEntity> FindAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            var query = SpecificationQueryBuilder.BuildQuery(EntitySet, specification);

            var result = await query.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }

        public override async Task<TOut> FindAsync<TOut>(ISpecification<TEntity, TOut> specification, CancellationToken cancellationToken = default)
        {
            var query = SpecificationQueryBuilder.BuildQuery(EntitySet, specification);

            var result = await query.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }

        public override async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            var query = SpecificationQueryBuilder.BuildQuery(EntitySet, specification);

            var results = await query.ToListAsync(cancellationToken).ConfigureAwait(false);
            return results;
        }

        public override async Task<IReadOnlyList<TOut>> ListAsync<TOut>(ISpecification<TEntity, TOut> specification, CancellationToken cancellationToken = default)
        {
            var query = SpecificationQueryBuilder.BuildQuery(EntitySet, specification);

            var results = await query.ToListAsync(cancellationToken).ConfigureAwait(false);
            return results;
        }

        public override async Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            var query = SpecificationQueryBuilder.BuildQuery(EntitySet, specification, SpecificationQueryBuilderOptions.IgnoreOrderBy);

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