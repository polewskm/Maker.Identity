using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Maker.Identity.Contracts;
using Maker.Identity.Contracts.Entities;
using Maker.Identity.Contracts.Repositories;
using Maker.Identity.Contracts.Specifications;
using Microsoft.AspNetCore.Identity;

namespace Maker.Identity.Data.Stores
{
    public class RoleStore : StoreBase, IRoleClaimStore<Role>
    {
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<RoleClaim> _roleClaimRepository;

        public RoleStore(IdentityErrorDescriber describer, IUnitOfWork unitOfWork)
            : base(describer, unitOfWork)
        {
            _roleRepository = unitOfWork.GetRepository<Role>();
            _roleClaimRepository = unitOfWork.GetRepository<RoleClaim>();
        }

        /// <summary>
        /// Creates an entity representing a role claim.
        /// </summary>
        /// <param name="role">The associated role.</param>
        /// <param name="claim">The associated claim.</param>
        /// <returns>The role claim entity.</returns>
        protected virtual RoleClaim CreateRoleClaim(Role role, Claim claim)
            => new RoleClaim
            {
                RoleId = role.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
            };

        #region IRoleStore Members

        /// <inheritdoc/>
        public virtual async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            await _roleRepository.AddAsync(role, cancellationToken).ConfigureAwait(false);

            return await TrySaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            await _roleRepository.UpdateAsync(role, cancellationToken).ConfigureAwait(false);

            return await TrySaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            await _roleRepository.RemoveAsync(role, cancellationToken).ConfigureAwait(false);

            return await TrySaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(ConvertToStringId(role.Id));
        }

        /// <inheritdoc/>
        public virtual Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(role.Name);
        }

        /// <inheritdoc/>
        public virtual Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            role.Name = roleName;

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(role.NormalizedName);
        }

        /// <inheritdoc/>
        public virtual Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
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
        public virtual async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            if (roleId == null)
                throw new ArgumentNullException(nameof(roleId));

            var id = ConvertFromStringId(roleId, nameof(roleId));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var specification = new Specification<Role>
            {
                Criteria = role => role.Id == id
            };
            return await _roleRepository.FindAsync(specification, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            if (normalizedRoleName == null)
                throw new ArgumentNullException(nameof(normalizedRoleName));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var specification = new Specification<Role>
            {
                Criteria = role => role.NormalizedName == normalizedRoleName
            };
            return await _roleRepository.FindAsync(specification, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region IRoleClaimStore Members

        /// <inheritdoc/>
        public virtual async Task<IList<Claim>> GetClaimsAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var specification = new Specification<RoleClaim, Claim>
            {
                Criteria = roleClaim => roleClaim.RoleId == role.Id,
                Projection = roleClaim => new Claim(roleClaim.ClaimType, roleClaim.ClaimValue)
            };
            var results = await _roleClaimRepository.ListAsync(specification, cancellationToken).ConfigureAwait(false);
            return results.ToList();
        }

        /// <inheritdoc/>
        public virtual async Task AddClaimAsync(Role role, Claim claim, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var newRoleClaim = CreateRoleClaim(role, claim);

            await _roleClaimRepository.AddAsync(newRoleClaim, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task RemoveClaimAsync(Role role, Claim claim, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var specification = new Specification<RoleClaim>
            {
                Criteria = roleClaim => roleClaim.RoleId == role.Id && roleClaim.ClaimType == claim.Type && roleClaim.ClaimValue == claim.Value
            };
            await _roleClaimRepository.RemoveAsync(specification, cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}