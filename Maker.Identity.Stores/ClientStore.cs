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
    public interface IClientStore : IStore
    {
        /// <summary>
        /// Finds the client which has the specified ID as an asynchronous operation.
        /// </summary>
        /// <param name="clientId">The client ID to look for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that result of the look up.</returns>
        Task<Client> FindByIdAsync(string clientId, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new client in a store as an asynchronous operation.
        /// </summary>
        /// <param name="client">The client to create in the store.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous operation.</returns>
        Task<IdentityResult> CreateAsync(Client client, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a client in a store as an asynchronous operation.
        /// </summary>
        /// <param name="client">The client to update in the store.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous operation.</returns>
        Task<IdentityResult> UpdateAsync(Client client, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a client from the store as an asynchronous operation.
        /// </summary>
        /// <param name="client">The client to delete from the store.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous operation.</returns>
        Task<IdentityResult> DeleteAsync(Client client, CancellationToken cancellationToken);

        Task<IEnumerable<KeyValuePair<string, string>>> GetTagsAsync(string clientId, CancellationToken cancellationToken);

        Task UpdateTagsAsync(string clientId, IEnumerable<KeyValuePair<string, string>> tags, CancellationToken cancellationToken);
    }

    public class ClientStore : StoreBase<MakerDbContext, Client, ClientBase, ClientHistory>, IClientStore
    {
        private static readonly Func<Client, Expression<Func<ClientHistory, bool>>> RetirePredicateFactory =
            client => history => history.ClientId == client.ClientId && history.RetiredWhen == Constants.MaxDateTimeOffset;

        public ClientStore(MakerDbContext context, IdentityErrorDescriber describer = null)
            : base(context, RetirePredicateFactory, describer)
        {
            // nothing
        }

        public virtual Task<Client> FindByIdAsync(string clientId, CancellationToken cancellationToken)
        {
            return base.FindByIdAsync(clientId, cancellationToken);
        }

        public virtual async Task<IEnumerable<KeyValuePair<string, string>>> GetTagsAsync(string clientId, CancellationToken cancellationToken)
        {
            return await Context.ClientTags
                .Where(_ => _.ClientId == clientId)
                .Select(tag => new KeyValuePair<string, string>(tag.Key, tag.Value))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public virtual async Task UpdateTagsAsync(string clientId, IEnumerable<KeyValuePair<string, string>> tags, CancellationToken cancellationToken)
        {
            ClientTag TagFactory() => new ClientTag { ClientId = clientId };
            Expression<Func<ClientTagHistory, bool>> RetirePredicateFactory(ClientTag tag) => history =>
                history.ClientId == tag.ClientId
                && history.NormalizedKey == tag.NormalizedKey
                && history.RetiredWhen == Constants.MaxDateTimeOffset;

            var existingTags = await Context.ClientTags
                .Where(_ => _.ClientId == clientId)
                .ToDictionaryAsync(_ => _.NormalizedKey, _ => _, StringComparer.Ordinal, cancellationToken)
                .ConfigureAwait(false);

            var store = new TagStore<MakerDbContext, ClientTag, ClientTagBase, ClientTagHistory>(Context, RetirePredicateFactory, TagFactory);

            await store.UpdateTagsAsync(tags, existingTags, cancellationToken).ConfigureAwait(false);
        }

    }
}