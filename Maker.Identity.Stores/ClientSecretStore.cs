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
    public interface IClientSecretStore
    {
        Task<IEnumerable<Secret>> GetSecretsAsync(long clientId, CancellationToken cancellationToken = default);

        Task AddSecretAsync(long clientId, long secretId, CancellationToken cancellationToken = default);

        Task RemoveSecretAsync(long clientId, long secretId, CancellationToken cancellationToken = default);
    }

    public class ClientSecretStore : ClientSecretStore<MakerDbContext>
    {
        public ClientSecretStore(MakerDbContext context, IdentityErrorDescriber describer, ISystemClock systemClock)
            : base(context, describer, systemClock)
        {
            // nothing
        }
    }

    public class ClientSecretStore<TContext> : StoreBase<TContext, ClientSecret, ClientSecretBase, ClientSecretHistory>, IClientSecretStore
        where TContext : DbContext, IClientSecretDbContext
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly Func<ClientSecret, Expression<Func<ClientSecretHistory, bool>>> RetirePredicateFactory =
            client => history => history.ClientId == client.ClientId && history.SecretId == client.SecretId && history.RetiredWhenUtc == Constants.MaxDateTime;

        public ClientSecretStore(TContext context, IdentityErrorDescriber describer, ISystemClock systemClock)
            : base(context, describer, systemClock, RetirePredicateFactory)
        {
            // nothing
        }

        #region IClientSecretStore Members

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<Secret>> GetSecretsAsync(long clientId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var query = from clientSecret in Context.ClientSecrets
                        join secret in Context.Secrets on clientSecret.SecretId equals secret.SecretId
                        where clientSecret.ClientId == clientId
                        select secret;

            return await query.ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task AddSecretAsync(long clientId, long secretId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var newClientSecret = new ClientSecret
            {
                ClientId = clientId,
                SecretId = secretId,
            };

            await CreateAsync(newClientSecret, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task RemoveSecretAsync(long clientId, long secretId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var clientSecret = await Context.ClientSecrets
                .SingleOrDefaultAsync(_ => _.ClientId == clientId && _.SecretId == secretId, cancellationToken)
                .ConfigureAwait(false);

            if (clientSecret == null)
                return;

            await DeleteAsync(clientSecret, cancellationToken).ConfigureAwait(false);
        }

        #endregion

    }
}