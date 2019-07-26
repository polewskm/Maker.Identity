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
    public interface IUserSecretStore
    {
        Task<IEnumerable<Secret>> GetSecretsAsync(long userId, CancellationToken cancellationToken = default);

        Task<IEnumerable<UserSecret>> GetActiveSecretsAsync(CancellationToken cancellationToken = default);

        Task AddSecretAsync(long userId, long secretId, CancellationToken cancellationToken = default);

        Task RemoveSecretAsync(long userId, long secretId, CancellationToken cancellationToken = default);
    }

    public class UserSecretStore : UserSecretStore<MakerDbContext>
    {
        public UserSecretStore(MakerDbContext context, IdentityErrorDescriber describer, ISystemClock systemClock)
            : base(context, describer, systemClock)
        {
            // nothing
        }
    }

    public class UserSecretStore<TContext> : UserSecretStore<TContext, User, UserBase, UserHistory>
        where TContext : DbContext, IUserSecretDbContext<User, UserBase, UserHistory>
    {
        public UserSecretStore(TContext context, IdentityErrorDescriber describer, ISystemClock systemClock)
            : base(context, describer, systemClock)
        {
            // nothing
        }
    }

    public class UserSecretStore<TContext, TUser, TUserBase, TUserHistory> :
        StoreBase<TContext, UserSecret, UserSecretBase, UserSecretHistory>,
        IUserSecretStore

        where TContext : DbContext, IUserSecretDbContext<TUser, TUserBase, TUserHistory>

        where TUser : class, TUserBase, ISupportConcurrencyToken
        where TUserBase : class, IUserBase, ISupportAssign<TUserBase>
        where TUserHistory : class, TUserBase, IHistoryEntity<TUserBase>, new()
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly Func<UserSecret, Expression<Func<UserSecretHistory, bool>>> RetirePredicateFactory =
            user => history => history.UserId == user.UserId && history.SecretId == user.SecretId && history.RetiredWhenUtc == Constants.MaxDateTime;

        public UserSecretStore(TContext context, IdentityErrorDescriber describer, ISystemClock systemClock)
            : base(context, describer, systemClock, RetirePredicateFactory)
        {
            // nothing
        }

        #region IUserSecretStore Members

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<Secret>> GetSecretsAsync(long userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var query = from userSecret in Context.UserSecrets
                        join secret in Context.Secrets on userSecret.SecretId equals secret.SecretId
                        where userSecret.UserId == userId
                        select secret;

            return await query.ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<UserSecret>> GetActiveSecretsAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var query = from userSecret in Context.UserSecrets
                        join user in Context.Users on userSecret.UserId equals user.UserId
                        // ReSharper disable once RedundantBoolCompare
                        where user.IsActive == true
                        select userSecret;

            return await query.ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task AddSecretAsync(long userId, long secretId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var newUserSecret = new UserSecret
            {
                UserId = userId,
                SecretId = secretId,
            };

            await CreateAsync(newUserSecret, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task RemoveSecretAsync(long userId, long secretId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var userSecret = await Context.UserSecrets
                .SingleOrDefaultAsync(_ => _.UserId == userId && _.SecretId == secretId, cancellationToken)
                .ConfigureAwait(false);

            if (userSecret == null)
                return;

            await DeleteAsync(userSecret, cancellationToken).ConfigureAwait(false);
        }

        #endregion

    }
}