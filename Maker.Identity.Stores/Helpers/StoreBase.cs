using System;
using System.Collections.Generic;
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

	public abstract class StoreBase : IStore
	{
		protected MakerDbContext Context { get; }

		public bool AutoSaveChanges { get; set; } = true;

		/// <summary>
		/// Gets or sets the <see cref="IdentityErrorDescriber"/> for any error that occurred with the current operation.
		/// </summary>
		public IdentityErrorDescriber ErrorDescriber { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context">The context used to access the store.</param>
		/// <param name="describer">The <see cref="IdentityErrorDescriber"/> used to describe store errors.</param>
		protected StoreBase(MakerDbContext context, IdentityErrorDescriber describer = null)
		{
			Context = context ?? throw new ArgumentNullException(nameof(context));
			ErrorDescriber = describer ?? new IdentityErrorDescriber();
		}

		public virtual Task SaveChangesAsync(CancellationToken cancellationToken)
		{
			return AutoSaveChanges ? Context.SaveChangesAsync(cancellationToken) : Task.CompletedTask;
		}
	}

	public abstract class StoreBase<TEntity, TBase, THistory> : StoreBase
		where TBase : ISupportAssign<TBase>
		where TEntity : class, TBase, new()
		where THistory : class, TBase, IHistoryEntity<TBase>, new()
	{
		private readonly DateTimeOffset _now = DateTimeOffset.Now;
		private readonly Func<TEntity, Expression<Func<THistory, bool>>> _retirePredicateFactory;

		protected StoreBase(MakerDbContext context, Func<TEntity, Expression<Func<THistory, bool>>> retirePredicateFactory, IdentityErrorDescriber describer = null)
			: base(context, describer)
		{
			_retirePredicateFactory = retirePredicateFactory ?? throw new ArgumentNullException(nameof(retirePredicateFactory));
		}

		private Task CreateHistoryAsync(TEntity entity, CancellationToken cancellationToken)
		{
			var newHistory = new THistory
			{
				CreatedWhen = _now,
				RetiredWhen = Constants.MaxDateTimeOffset,
			};
			newHistory.Assign(entity);

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
				item.RetiredWhen = _now;

				Context.Set<THistory>().Update(item);
			}
		}

		public virtual async Task<TEntity> FindByIdAsync(object keyValue, CancellationToken cancellationToken)
		{
			return await Context.Set<TEntity>().FindAsync(new[] { keyValue }, cancellationToken).ConfigureAwait(false);
		}

		public virtual async Task<TEntity> FindByIdAsync(object[] keyValues, CancellationToken cancellationToken)
		{
			return await Context.Set<TEntity>().FindAsync(keyValues, cancellationToken).ConfigureAwait(false);
		}

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
				Context.Set<TEntity>().Add(entity);

				await CreateHistoryAsync(entity, cancellationToken).ConfigureAwait(false);
			}

			await SaveChangesAsync(cancellationToken).ConfigureAwait(false);

			return IdentityResult.Success;
		}

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

				Context.Set<TEntity>().Update(entity);

				await RetireHistoryAsync(entity, cancellationToken).ConfigureAwait(false);

				await CreateHistoryAsync(entity, cancellationToken).ConfigureAwait(false);
			}

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

		public virtual Task<IdentityResult> DeleteAsync(TEntity entity, CancellationToken cancellationToken)
		{
			return DeleteAsync(new[] { entity }, cancellationToken);
		}

		public virtual async Task<IdentityResult> DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
		{
			foreach (var entity in entities)
			{
				Context.Set<TEntity>().Remove(entity);

				await RetireHistoryAsync(entity, cancellationToken).ConfigureAwait(false);
			}

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

	}
}