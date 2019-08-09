using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Maker.Identity.Stores.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Stores.Helpers
{
    public interface IStore
    {
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }

    public abstract class StoreBase<TContext, TEntity> : IStore
        where TContext : DbContext
        where TEntity : class
    {
        public bool AutoSaveChanges { get; set; } = true;

        protected TContext Context { get; }

        /// <summary>
        /// Gets or sets the <see cref="IdentityErrorDescriber"/> for any error that occurred with the current operation.
        /// </summary>
        protected IdentityErrorDescriber ErrorDescriber { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreBase{TContext,TEntity}"/> class.
        /// </summary>
        /// <param name="context">The context used to access the store.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/> used to describe store errors.</param>
        protected StoreBase(TContext context, IdentityErrorDescriber describer = null)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            ErrorDescriber = describer ?? new IdentityErrorDescriber();
        }

        /// <summary>
        /// Converts the provided <paramref name="value"/> to a strongly typed <see cref="long"/>.
        /// </summary>
        protected virtual long ConvertFromStringId(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
                return 0L;

            if (!long.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result))
                throw new ArgumentException("Input string was not in a correct format.", paramName);

            return result;
        }

        /// <summary>
        /// Converts the provided <paramref name="value"/> to its string representation.
        /// </summary>
        protected virtual string ConvertToStringId(long value)
        {
            return value == 0L ? null : value.ToString();
        }

        public virtual Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return AutoSaveChanges ? Context.SaveChangesAsync(cancellationToken) : Task.CompletedTask;
        }

        protected virtual async Task<IdentityResult> TrySaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }
            return IdentityResult.Success;
        }

        #region Create Members

        public virtual Task<IdentityResult> CreateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return CreateAsync(new[] { entity }, cancellationToken);
        }

        public virtual async Task<IdentityResult> CreateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
            {
                await BeforeCreateAsync(entity, cancellationToken).ConfigureAwait(false);

                Context.Set<TEntity>().Add(entity);

                await AfterCreateAsync(entity, cancellationToken).ConfigureAwait(false);
            }

            return await TrySaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        protected virtual Task BeforeCreateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected virtual Task AfterCreateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Update Members

        public virtual Task<IdentityResult> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return UpdateAsync(new[] { entity }, cancellationToken);
        }

        public virtual async Task<IdentityResult> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
            {
                if (!Context.Set<TEntity>().Local.Contains(entity))
                    Context.Set<TEntity>().Attach(entity);

                if (entity is ISupportConcurrencyToken supportConcurrencyToken)
                    supportConcurrencyToken.ConcurrencyStamp = Guid.NewGuid().ToString();

                await BeforeUpdateAsync(entity, cancellationToken).ConfigureAwait(false);

                Context.Set<TEntity>().Update(entity);

                await AfterUpdateAsync(entity, cancellationToken).ConfigureAwait(false);
            }

            return await TrySaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        protected virtual Task BeforeUpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected virtual Task AfterUpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Delete Members

        public virtual Task<IdentityResult> DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return DeleteAsync(new[] { entity }, cancellationToken);
        }

        public virtual async Task<IdentityResult> DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            foreach (var entity in entities)
            {
                await BeforeDeleteAsync(entity, cancellationToken).ConfigureAwait(false);

                Context.Set<TEntity>().Remove(entity);

                await AfterDeleteAsync(entity, cancellationToken).ConfigureAwait(false);
            }

            return await TrySaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        protected virtual Task BeforeDeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected virtual Task AfterDeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        #endregion

    }

    public abstract class StoreBase<TContext, TEntity, TBase, THistory> : StoreBase<TContext, TEntity>
        where TContext : DbContext
        where TBase : ISupportAssign<TBase>
        where TEntity : class, TBase
        where THistory : class, TBase, IHistoryEntity<TBase>, new()
    {
        private readonly Func<TEntity, Expression<Func<THistory, bool>>> _retirePredicateFactory;

        protected ISystemClock SystemClock { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreBase{TContext,TEntity,TBase,THistory}"/> class.
        /// </summary>
        /// <param name="context">The context used to access the store.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/> used to describe store errors.</param>
        /// <param name="systemClock"><see cref="ISystemClock"/></param>
        /// <param name="retirePredicateFactory"></param>
        protected StoreBase(TContext context, IdentityErrorDescriber describer, ISystemClock systemClock, Func<TEntity, Expression<Func<THistory, bool>>> retirePredicateFactory)
            : base(context, describer)
        {
            _retirePredicateFactory = retirePredicateFactory ?? throw new ArgumentNullException(nameof(retirePredicateFactory));
            SystemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        }

        private Task CreateHistoryAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var newHistory = new THistory
            {
                CreatedWhenUtc = SystemClock.UtcNow.UtcDateTime,
                RetiredWhenUtc = Constants.MaxDateTime,
            };
            newHistory.Assign(entity);

            cancellationToken.ThrowIfCancellationRequested();

            Context.Set<THistory>().Add(newHistory);

            return Task.CompletedTask;
        }

        private async Task RetireHistoryAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var predicate = _retirePredicateFactory(entity);

            var itemsToRetire = await Context
                .Set<THistory>()
                .Where(predicate)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            foreach (var item in itemsToRetire)
            {
                item.RetiredWhenUtc = SystemClock.UtcNow.UtcDateTime;

                cancellationToken.ThrowIfCancellationRequested();

                Context.Set<THistory>().Update(item);
            }
        }

        protected override async Task AfterCreateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await base.AfterCreateAsync(entity, cancellationToken).ConfigureAwait(false);

            await CreateHistoryAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task AfterUpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await base.AfterUpdateAsync(entity, cancellationToken).ConfigureAwait(false);

            await RetireHistoryAsync(entity, cancellationToken).ConfigureAwait(false);

            await CreateHistoryAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task AfterDeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await RetireHistoryAsync(entity, cancellationToken).ConfigureAwait(false);

            await base.AfterDeleteAsync(entity, cancellationToken).ConfigureAwait(false);
        }

    }
}