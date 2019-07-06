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
    public class RoleStore :
        StoreBase<Role, RoleBase, RoleHistory>,
        IQueryableRoleStore<Role>,
        IRoleClaimStore<Role>
    {
        private static readonly Func<Role, Expression<Func<RoleHistory, bool>>> RetirePredicateFactory =
            role => history => history.RoleId == role.RoleId && history.RetiredWhen == Constants.MaxDateTimeOffset;

        private bool _disposed;

        public RoleStore(MakerDbContext context, IdentityErrorDescriber describer = null)
            : base(context, RetirePredicateFactory, describer)
        {
            // nothing
        }

        /// <summary>
        /// Creates an entity representing a role claim.
        /// </summary>
        /// <param name="role">The associated role.</param>
        /// <param name="claim">The associated claim.</param>
        /// <returns>The role claim entity.</returns>
        protected virtual RoleClaim CreateRoleClaim(Role role, Claim claim)
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

        public virtual Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(role.RoleId);
        }

        public virtual Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(role.Name);
        }

        public virtual Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            role.Name = roleName;

            return Task.CompletedTask;
        }

        public virtual Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(role.NormalizedName);
        }

        public virtual Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            role.NormalizedName = normalizedName;

            return Task.CompletedTask;
        }

        public virtual async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            if (roleId == null)
                throw new ArgumentNullException(nameof(roleId));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return await Context.Roles.FirstOrDefaultAsync(_ => _.RoleId == roleId, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            if (normalizedRoleName == null)
                throw new ArgumentNullException(nameof(normalizedRoleName));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return await Context.Roles.FirstOrDefaultAsync(_ => _.NormalizedName == normalizedRoleName, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region IQueryableRoleStore Members

        public virtual IQueryable<Role> Roles => Context.Roles;

        #endregion

        #region IRoleClaimStore Members

        public virtual async Task<IList<Claim>> GetClaimsAsync(Role role, CancellationToken cancellationToken = new CancellationToken())
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

        public virtual async Task AddClaimAsync(Role role, Claim claim, CancellationToken cancellationToken = new CancellationToken())
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var newRoleClaim = CreateRoleClaim(role, claim);

            var store = new RoleClaimStore(Context, ErrorDescriber);

            await store.CreateAsync(newRoleClaim, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task RemoveClaimAsync(Role role, Claim claim, CancellationToken cancellationToken = new CancellationToken())
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var store = new RoleClaimStore(Context, ErrorDescriber);

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