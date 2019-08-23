using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Maker.Identity.Contracts.Entities;
using Maker.Identity.Contracts.Repositories;
using Maker.Identity.Contracts.Specifications;

namespace Maker.Identity.Data.Repositories
{
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        public Type EntityType => typeof(TEntity);

        public abstract Task<TEntity> FindAsync(IQueryConfiguration<TEntity> queryConfiguration, CancellationToken cancellationToken = default);

        public abstract Task<TOut> FindAsync<TOut>(IQueryConfiguration<TEntity, TOut> queryConfiguration, CancellationToken cancellationToken = default) where TOut : class;

        public abstract Task<IReadOnlyList<TEntity>> ListAsync(IQueryConfiguration<TEntity> queryConfiguration, CancellationToken cancellationToken = default);

        public abstract Task<IReadOnlyList<TOut>> ListAsync<TOut>(IQueryConfiguration<TEntity, TOut> queryConfiguration, CancellationToken cancellationToken = default) where TOut : class;

        public abstract Task<int> CountAsync(IQueryConfiguration<TEntity> queryConfiguration, CancellationToken cancellationToken = default);

        #region Add Members

        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await BeforeAddAsync(entity, cancellationToken).ConfigureAwait(false);

            await CoreAddAsync(entity, cancellationToken).ConfigureAwait(false);

            await AfterAddAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected abstract Task CoreAddAsync(TEntity entity, CancellationToken cancellationToken);

        protected virtual Task BeforeAddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            if (entity is ISupportConcurrencyToken supportConcurrencyToken && supportConcurrencyToken.ConcurrencyStamp == null)
                supportConcurrencyToken.ConcurrencyStamp = Guid.NewGuid().ToString();

            return Task.CompletedTask;
        }

        protected virtual Task AfterAddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Update Members

        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await BeforeUpdateAsync(entity, cancellationToken).ConfigureAwait(false);

            await CoreUpdateAsync(entity, cancellationToken).ConfigureAwait(false);

            await AfterUpdateAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected abstract Task CoreUpdateAsync(TEntity entity, CancellationToken cancellationToken);

        protected virtual Task BeforeUpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            if (entity is ISupportConcurrencyToken supportConcurrencyToken)
                supportConcurrencyToken.ConcurrencyStamp = Guid.NewGuid().ToString();

            return Task.CompletedTask;
        }

        protected virtual Task AfterUpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Remove Members

        public virtual async Task RemoveAsync(IQueryConfiguration<TEntity> queryConfiguration, CancellationToken cancellationToken = default)
        {
            var results = await ListAsync(queryConfiguration, cancellationToken).ConfigureAwait(false);
            foreach (var entity in results)
            {
                await RemoveAsync(entity, cancellationToken).ConfigureAwait(false);
            }
        }

        public virtual async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await BeforeRemoveAsync(entity, cancellationToken).ConfigureAwait(false);

            await CoreRemoveAsync(entity, cancellationToken).ConfigureAwait(false);

            await AfterRemoveAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected abstract Task CoreRemoveAsync(TEntity entity, CancellationToken cancellationToken);

        protected virtual Task BeforeRemoveAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected virtual Task AfterRemoveAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        #endregion

    }
}