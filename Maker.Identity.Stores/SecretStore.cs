using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Maker.Identity.Stores.Entities;
using Maker.Identity.Stores.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Stores
{
    public interface ISecretStore : IStore
    {
        /// <summary>
        /// Finds the secret which has the specified ID as an asynchronous operation.
        /// </summary>
        /// <param name="secretId">The secret ID to look for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that result of the look up.</returns>
        Task<Secret> FindByIdAsync(long secretId, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new secret in a store as an asynchronous operation.
        /// </summary>
        /// <param name="secret">The secret to create in the store.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous operation.</returns>
        Task<IdentityResult> CreateAsync(Secret secret, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a secret in a store as an asynchronous operation.
        /// </summary>
        /// <param name="secret">The secret to update in the store.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous operation.</returns>
        Task<IdentityResult> UpdateAsync(Secret secret, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a secret from the store as an asynchronous operation.
        /// </summary>
        /// <param name="secret">The secret to delete from the store.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous operation.</returns>
        Task<IdentityResult> DeleteAsync(Secret secret, CancellationToken cancellationToken);

        Task<IEnumerable<KeyValuePair<string, string>>> GetTagsAsync(long secretId, CancellationToken cancellationToken);

        Task UpdateTagsAsync(long secretId, IEnumerable<KeyValuePair<string, string>> tags, CancellationToken cancellationToken);
    }

    public class SecretStore : SecretStore<MakerDbContext>
    {
        public SecretStore(MakerDbContext context, IdentityErrorDescriber describer = null)
            : base(context, describer)
        {
            // nothing
        }
    }

    public class SecretStore<TContext> : StoreBase<TContext, Secret, SecretBase, SecretHistory>, ISecretStore
        where TContext : DbContext, ISecretDbContext
    {
        private static readonly Func<Secret, Expression<Func<SecretHistory, bool>>> RetirePredicateFactory =
            client => history => history.SecretId == client.SecretId && history.RetiredWhenUtc == Constants.MaxDateTime;

        public SecretStore(TContext context, IdentityErrorDescriber describer = null)
            : base(context, RetirePredicateFactory, describer)
        {
            // nothing
        }

        public virtual async Task<Secret> FindByIdAsync(long secretId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await Context.Secrets
                .FirstOrDefaultAsync(_ => _.SecretId == secretId, cancellationToken)
                .ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<KeyValuePair<string, string>>> GetTagsAsync(long secretId, CancellationToken cancellationToken)
        {
            return await Context.SecretTags
                .Where(_ => _.SecretId == secretId)
                .Select(tag => new KeyValuePair<string, string>(tag.Key, tag.Value))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public virtual async Task UpdateTagsAsync(long secretId, IEnumerable<KeyValuePair<string, string>> tags, CancellationToken cancellationToken)
        {
            SecretTag TagFactory() => new SecretTag { SecretId = secretId };
            Expression<Func<SecretTagHistory, bool>> RetirePredicateFactory(SecretTag tag) => history =>
                history.SecretId == tag.SecretId
                && history.NormalizedKey == tag.NormalizedKey
                && history.RetiredWhenUtc == Constants.MaxDateTime;

            var existingTags = await Context.SecretTags
                .Where(_ => _.SecretId == secretId)
                .ToDictionaryAsync(_ => _.NormalizedKey, _ => _, StringComparer.Ordinal, cancellationToken)
                .ConfigureAwait(false);

            var store = new TagStore<TContext, SecretTag, SecretTagBase, SecretTagHistory>(Context, RetirePredicateFactory, TagFactory);

            await store.UpdateTagsAsync(tags, existingTags, cancellationToken).ConfigureAwait(false);
        }

    }
}