using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Maker.Identity.Stores.Entities;
using Maker.Identity.Stores.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Stores
{
    public class RoleStore : RoleStore<MakerDbContext>
    {
        public RoleStore(MakerDbContext context, IdentityErrorDescriber describer, ISystemClock systemClock)
            : base(context, describer, systemClock)
        {
            // nothing
        }
    }

    public class RoleStore<TContext> : RoleStore<TContext, Role, RoleBase, RoleHistory>
        where TContext : DbContext, IRoleDbContext<Role, RoleBase, RoleHistory>
    {
        public RoleStore(TContext context, IdentityErrorDescriber describer, ISystemClock systemClock)
            : base(context, describer, systemClock)
        {
            // nothing
        }
    }

    public class RoleStore<TContext, TRole, TRoleBase, TRoleHistory> :
        StoreBase<TContext, TRole, TRoleBase, TRoleHistory>,
        IQueryableRoleStore<TRole>,
        IRoleClaimStore<TRole>

        where TContext : DbContext, IRoleDbContext<TRole, TRoleBase, TRoleHistory>

        where TRole : class, TRoleBase, ISupportConcurrencyToken
        where TRoleBase : class, IRoleBase, ISupportAssign<TRoleBase>
        where TRoleHistory : class, TRoleBase, IHistoryEntity<TRoleBase>, new()
    {
        private static readonly Func<TRole, Expression<Func<TRoleHistory, bool>>> RetirePredicateFactory =
            role => history => history.RoleId == role.RoleId && history.RetiredWhenUtc == Constants.MaxDateTime;

        private bool _disposed;

        public RoleStore(TContext context, IdentityErrorDescriber describer, ISystemClock systemClock)
            : base(context, describer, systemClock, RetirePredicateFactory)
        {
            // nothing
        }

        /// <summary>
        /// Creates an entity representing a role claim.
        /// </summary>
        /// <param name="role">The associated role.</param>
        /// <param name="claim">The associated claim.</param>
        /// <returns>The role claim entity.</returns>
        protected virtual RoleClaim CreateRoleClaim(TRole role, Claim claim)
            => new RoleClaim { RoleId = role.RoleId, ClaimType = claim.Type, ClaimValue = claim.Value };

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _disposed = true;
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        #endregion

        #region IRoleStore Members

        /// <inheritdoc/>
        public virtual Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(ConvertIdToString(role.RoleId));
        }

        /// <inheritdoc/>
        public virtual Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(role.Name);
        }

        /// <inheritdoc/>
        public virtual Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            role.Name = roleName;

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(role.NormalizedName);
        }

        /// <inheritdoc/>
        public virtual Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            if (normalizedName == null)
                throw new ArgumentNullException(nameof(normalizedName));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            role.NormalizedName = normalizedName;

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            if (roleId == null)
                throw new ArgumentNullException(nameof(roleId));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var id = ConvertIdFromString(roleId);

            return await Context.Roles
                .FirstOrDefaultAsync(_ => _.RoleId == id, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            if (normalizedRoleName == null)
                throw new ArgumentNullException(nameof(normalizedRoleName));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return await Context.Roles
                .FirstOrDefaultAsync(_ => _.NormalizedName == normalizedRoleName, cancellationToken)
                .ConfigureAwait(false);
        }

        #endregion

        #region IQueryableRoleStore Members

        /// <inheritdoc/>
        public virtual IQueryable<TRole> Roles => Context.Roles;

        #endregion

        #region IRoleClaimStore Members

        /// <inheritdoc/>
        public virtual async Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return await Context.RoleClaims
                .Where(_ => _.RoleId == role.RoleId)
                .Select(_ => new Claim(_.ClaimType, _.ClaimValue))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var newRoleClaim = CreateRoleClaim(role, claim);

            var store = new RoleClaimStore<TContext>(Context, ErrorDescriber, SystemClock);

            await store.CreateAsync(newRoleClaim, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var store = new RoleClaimStore<TContext>(Context, ErrorDescriber, SystemClock);

            var claims = await Context.RoleClaims
                .Where(_ =>
                    _.RoleId == role.RoleId &&
                    _.ClaimValue == claim.Value &&
                    _.ClaimType == claim.Type)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            await store.DeleteAsync(claims, cancellationToken).ConfigureAwait(false);
        }

        #endregion

    }
}