using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Maker.Identity.Contracts;
using Maker.Identity.Contracts.Entities;
using Maker.Identity.Contracts.Repositories;
using Maker.Identity.Contracts.Specifications;
using Maker.Identity.Stores.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Maker.Identity.Stores
{
    public class UserStore : StoreBase,
        IUserRoleStore<User>,
        IUserLoginStore<User>,
        IUserClaimStore<User>,
        IUserPasswordStore<User>,
        IUserSecurityStampStore<User>,
        IUserEmailStore<User>,
        IUserLockoutStore<User>,
        IUserPhoneNumberStore<User>,
        IUserTwoFactorStore<User>,
        IUserAuthenticationTokenStore<User>,
        IUserAuthenticatorKeyStore<User>,
        IUserTwoFactorRecoveryCodeStore<User>
    {
        private const string InternalLoginProvider = "[MakerIdentityUserStore]";
        private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
        private const string RecoveryCodeTokenName = "RecoveryCodes";

        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<UserToken> _userTokenRepository;
        private readonly IRepository<UserLogin> _userLoginRepository;
        private readonly IRepository<UserClaim> _userClaimRepository;
        private readonly IRepository<Role> _roleRepository;

        public UserStore(IdentityErrorDescriber describer, IUnitOfWork unitOfWork)
            : base(describer, unitOfWork)
        {
            _userRepository = unitOfWork.GetRepository<User>();
            _userRoleRepository = unitOfWork.GetRepository<UserRole>();
            _userTokenRepository = unitOfWork.GetRepository<UserToken>();
            _userLoginRepository = unitOfWork.GetRepository<UserLogin>();
            _userClaimRepository = unitOfWork.GetRepository<UserClaim>();
            _roleRepository = unitOfWork.GetRepository<Role>();
        }

        #region Factory Methods

        /// <summary>
        /// Called to create a new instance of a <see cref="UserClaim"/>.
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="claim">The associated claim.</param>
        /// <returns></returns>
        protected virtual UserClaim CreateUserClaim(User user, Claim claim)
            => new UserClaim
            {
                UserId = user.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            };

        /// <summary>
        /// Called to create a new instance of a <see cref="UserLogin"/>.
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="login">The associated login.</param>
        /// <returns></returns>
        protected virtual UserLogin CreateUserLogin(User user, UserLoginInfo login)
            => new UserLogin
            {
                UserId = user.Id,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName
            };

        /// <summary>
        /// Called to create a new instance of a <see cref="UserToken"/>.
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="loginProvider">The associated login provider.</param>
        /// <param name="name">The name of the user token.</param>
        /// <param name="value">The value of the user token.</param>
        /// <returns></returns>
        protected virtual UserToken CreateUserToken(User user, string loginProvider, string name, string value)
            => new UserToken
            {
                UserId = user.Id,
                LoginProvider = loginProvider,
                Name = name,
                Value = value
            };

        /// <summary>
        /// Called to create a new instance of a <see cref="UserRole"/>.
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="role">The associated role.</param>
        /// <returns></returns>
        protected virtual UserRole CreateUserRole(User user, Role role)
            => new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            };

        #endregion

        #region Search Methods

        /// <summary>
        /// Return a role with the normalized name if it exists.
        /// </summary>
        /// <param name="normalizedRoleName">The normalized role name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The role if it exists.</returns>
        protected virtual Task<Role> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            var query = Query<Role>.Build(configure => configure
                .Where(role => role.NormalizedName == normalizedRoleName));

            return _roleRepository.FindAsync(query, cancellationToken);
        }

        /// <summary>
        /// Return a user role for the userId and roleId if it exists.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="roleId">The role's id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user role if it exists.</returns>
        protected virtual Task<UserRole> FindUserRoleAsync(long userId, long roleId, CancellationToken cancellationToken)
        {
            var query = Query<UserRole>.Build(configure => configure
                .Where(userRole => userRole.UserId == userId && userRole.RoleId == roleId));

            return _userRoleRepository.FindAsync(query, cancellationToken);
        }

        /// <summary>
        /// Find a user token if it exists.
        /// </summary>
        /// <param name="user">The token owner.</param>
        /// <param name="loginProvider">The login provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user token if it exists.</returns>
        protected virtual Task<UserToken> FindTokenAsync(User user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            var query = Query<UserToken>.Build(configure => configure
                .Where(userToken => userToken.UserId == user.Id &&
                                    userToken.LoginProvider == loginProvider &&
                                    userToken.Name == name));

            return _userTokenRepository.FindAsync(query, cancellationToken);
        }

        #endregion

        #region IUserStore Members

        /// <inheritdoc/>
        public virtual async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            await _userRepository.AddAsync(user, cancellationToken).ConfigureAwait(false);

            return await TrySaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            await _userRepository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);

            return await TrySaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            await _userRepository.RemoveAsync(user, cancellationToken).ConfigureAwait(false);

            return await TrySaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(ConvertToStringId(user.Id));
        }

        /// <inheritdoc/>
        public virtual Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.UserName);
        }

        /// <inheritdoc/>
        public virtual Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.UserName = userName;

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.NormalizedUserName);
        }

        /// <inheritdoc/>
        public virtual Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.NormalizedUserName = normalizedName;

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var id = ConvertFromStringId(userId, nameof(userId));

            return await _userRepository.FindAsync(id, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            if (normalizedUserName == null)
                throw new ArgumentNullException(nameof(normalizedUserName));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var query = Query<User>.Build(configure => configure
                .Where(user => user.NormalizedUserName == normalizedUserName));

            return await _userRepository.FindAsync(query, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region IUserRoleStore Members

        /// <inheritdoc/>
        public virtual async Task AddToRoleAsync(User user, string normalizedRoleName, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(normalizedRoleName))
                throw new ArgumentNullException(nameof(normalizedRoleName));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var roleEntity = await FindRoleAsync(normalizedRoleName, cancellationToken).ConfigureAwait(false);
            if (roleEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Role {0} does not exist.", normalizedRoleName));
            }

            var newUserRole = CreateUserRole(user, roleEntity);

            await _userRoleRepository.AddAsync(newUserRole, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task RemoveFromRoleAsync(User user, string normalizedRoleName, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(normalizedRoleName))
                throw new ArgumentNullException(nameof(normalizedRoleName));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var query = Query<UserRole>.Build(configure => configure
                .Where(userRole => userRole.UserId == user.Id &&
                                   userRole.Role.NormalizedName == normalizedRoleName));

            await _userRoleRepository.RemoveAsync(query, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var query = Query<UserRole>.Build(configure => configure
                .Where(userRole => userRole.UserId == user.Id)
                .Select(userRole => userRole.Role.Name)
                .Output(output => output.Distinct()));

            var results = await _userRoleRepository.ListAsync(query, cancellationToken).ConfigureAwait(false);
            return results.ToList();
        }

        /// <inheritdoc/>
        public virtual async Task<bool> IsInRoleAsync(User user, string normalizedRoleName, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(normalizedRoleName))
                throw new ArgumentNullException(nameof(normalizedRoleName));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var query = Query<UserRole>.Build(configure => configure
                .Where(userRole => userRole.UserId == user.Id &&
                                   userRole.Role.NormalizedName == normalizedRoleName));

            var result = await _userRoleRepository.FindAsync(query, cancellationToken).ConfigureAwait(false);
            return result != null;
        }

        /// <inheritdoc/>
        public virtual async Task<IList<User>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(normalizedRoleName))
                throw new ArgumentNullException(nameof(normalizedRoleName));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var query = Query<UserRole>.Build(configure => configure
                .Where(userRole => userRole.Role.NormalizedName == normalizedRoleName)
                .Select(userRole => userRole.User));

            var results = await _userRoleRepository.ListAsync(query, cancellationToken).ConfigureAwait(false);
            return results.ToList();
        }

        #endregion

        #region IUserLoginStore Members

        /// <inheritdoc/>
        public virtual async Task AddLoginAsync(User user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (login == null)
                throw new ArgumentNullException(nameof(login));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var newUserLogin = CreateUserLogin(user, login);

            await _userLoginRepository.AddAsync(newUserLogin, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task RemoveLoginAsync(User user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var query = Query<UserLogin>.Build(configure => configure
                .Where(userLogin => userLogin.LoginProvider == loginProvider &&
                                    userLogin.ProviderKey == providerKey &&
                                    userLogin.UserId == user.Id));

            await _userLoginRepository.RemoveAsync(query, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<IList<UserLoginInfo>> GetLoginsAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var query = Query<UserLogin>.Build(configure => configure
                .Where(userLogin => userLogin.UserId == user.Id)
                .Select(userLogin => new UserLoginInfo(userLogin.LoginProvider, userLogin.ProviderKey, userLogin.ProviderDisplayName)));

            var results = await _userLoginRepository.ListAsync(query, cancellationToken).ConfigureAwait(false);
            return results.ToList();
        }

        /// <inheritdoc/>
        public virtual async Task<User> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var query = Query<UserLogin>.Build(configure => configure
                .Where(userLogin => userLogin.LoginProvider == loginProvider &&
                                    userLogin.ProviderKey == providerKey)
                .Select(userLogin => userLogin.User));

            return await _userLoginRepository.FindAsync(query, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region IUserClaimStore Members

        /// <inheritdoc/>
        public virtual async Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var query = Query<UserClaim>.Build(configure => configure
                .Where(userClaim => userClaim.UserId == user.Id)
                .Select(userClaim => new Claim(userClaim.ClaimType, userClaim.ClaimValue)));

            var results = await _userClaimRepository.ListAsync(query, cancellationToken).ConfigureAwait(false);
            return results.ToList();
        }

        /// <inheritdoc/>
        public virtual async Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var newClaims = claims.Select(claim => CreateUserClaim(user, claim));

            foreach (var newClaim in newClaims)
            {
                await _userClaimRepository.AddAsync(newClaim, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public virtual async Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (claim == null)
                throw new ArgumentNullException(nameof(claim));
            if (newClaim == null)
                throw new ArgumentNullException(nameof(newClaim));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var query = Query<UserClaim>.Build(configure => configure
                .Where(userClaim => userClaim.UserId == user.Id &&
                                    userClaim.ClaimType == claim.Type &&
                                    userClaim.ClaimValue == claim.Value));

            var matchedClaims = await _userClaimRepository.ListAsync(query, cancellationToken).ConfigureAwait(false);

            foreach (var matchedClaim in matchedClaims)
            {
                matchedClaim.ClaimType = newClaim.Type;
                matchedClaim.ClaimValue = newClaim.Value;

                await _userClaimRepository.UpdateAsync(matchedClaim, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public virtual async Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var claim in claims)
            {
                var query = Query<UserClaim>.Build(configure => configure
                    .Where(userClaim => userClaim.UserId == user.Id &&
                                        userClaim.ClaimType == claim.Type &&
                                        userClaim.ClaimValue == claim.Value));

                await _userClaimRepository.RemoveAsync(query, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public virtual async Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var query = Query<UserClaim>.Build(configure => configure
                .Where(userClaim => userClaim.ClaimType == claim.Type &&
                                    userClaim.ClaimValue == claim.Value)
                .Select(userClaim => userClaim.User));

            var results = await _userClaimRepository.ListAsync(query, cancellationToken).ConfigureAwait(false);
            return results.ToList();
        }

        #endregion

        #region IUserPasswordStore Members

        /// <inheritdoc/>
        public virtual Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.PasswordHash = passwordHash;

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.PasswordHash);
        }

        /// <inheritdoc/>
        public virtual Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.PasswordHash != null);
        }

        #endregion

        #region IUserSecurityStampStore Members

        /// <inheritdoc/>
        public virtual Task SetSecurityStampAsync(User user, string stamp, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.SecurityStamp = stamp;

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<string> GetSecurityStampAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.SecurityStamp);
        }

        #endregion

        #region IUserEmailStore Members

        /// <inheritdoc/>
        public virtual Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.Email = email;

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.Email);
        }

        /// <inheritdoc/>
        public virtual Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.EmailConfirmed);
        }

        /// <inheritdoc/>
        public virtual Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.EmailConfirmed = confirmed;

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.NormalizedEmail);
        }

        /// <inheritdoc/>
        public virtual Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.NormalizedEmail = normalizedEmail;

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            if (normalizedEmail == null)
                throw new ArgumentNullException(nameof(normalizedEmail));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var query = Query<User>.Build(configure => configure
                .Where(user => user.NormalizedEmail == normalizedEmail));

            return await _userRepository.FindAsync(query, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region IUserLockoutStore Members

        /// <inheritdoc/>
        public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.LockoutEndUtc?.ToDateTimeOffset(TimeSpan.Zero));
        }

        /// <inheritdoc/>
        public virtual Task SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.LockoutEndUtc = lockoutEnd?.UtcDateTime;

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<int> IncrementAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.AccessFailedCount++;

            return Task.FromResult(user.AccessFailedCount);
        }

        /// <inheritdoc/>
        public virtual Task ResetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.AccessFailedCount = 0;

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<int> GetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.AccessFailedCount);
        }

        /// <inheritdoc/>
        public virtual Task<bool> GetLockoutEnabledAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.LockoutEnabled);
        }

        /// <inheritdoc/>
        public virtual Task SetLockoutEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.LockoutEnabled = enabled;

            return Task.CompletedTask;
        }

        #endregion

        #region IUserPhoneNumberStore Members

        /// <inheritdoc/>
        public virtual Task SetPhoneNumberAsync(User user, string phoneNumber, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.PhoneNumber = phoneNumber;

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<string> GetPhoneNumberAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.PhoneNumber);
        }

        /// <inheritdoc/>
        public virtual Task<bool> GetPhoneNumberConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        /// <inheritdoc/>
        public virtual Task SetPhoneNumberConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.PhoneNumberConfirmed = confirmed;

            return Task.CompletedTask;
        }

        #endregion

        #region IUserTwoFactorStore Members

        /// <inheritdoc/>
        public virtual Task SetTwoFactorEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.TwoFactorEnabled = enabled;

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<bool> GetTwoFactorEnabledAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.TwoFactorEnabled);
        }

        #endregion

        #region IUserAuthenticationTokenStore Members

        /// <inheritdoc/>
        public virtual async Task SetTokenAsync(User user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var token = await FindTokenAsync(user, loginProvider, name, cancellationToken).ConfigureAwait(false);
            if (token == null)
            {
                var newToken = CreateUserToken(user, loginProvider, name, value);

                await _userTokenRepository.AddAsync(newToken, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                token.Value = value;

                await _userTokenRepository.UpdateAsync(token, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public virtual async Task RemoveTokenAsync(User user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var query = Query<UserToken>.Build(configure => configure
                .Where(userToken => userToken.UserId == user.Id &&
                                    userToken.LoginProvider == loginProvider &&
                                    userToken.Name == name));

            await _userTokenRepository.RemoveAsync(query, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<string> GetTokenAsync(User user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var token = await FindTokenAsync(user, loginProvider, name, cancellationToken).ConfigureAwait(false);
            return token?.Value;
        }

        #endregion

        #region IUserAuthenticatorKeyStore Members

        /// <inheritdoc/>
        public virtual Task SetAuthenticatorKeyAsync(User user, string key, CancellationToken cancellationToken)
            => SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);

        /// <inheritdoc/>
        public virtual Task<string> GetAuthenticatorKeyAsync(User user, CancellationToken cancellationToken)
            => GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);

        #endregion

        #region IUserTwoFactorRecoveryCodeStore Members

        private const string CodeDelimiter = ";";

        /// <inheritdoc/>
        public virtual Task ReplaceCodesAsync(User user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (recoveryCodes == null)
                throw new ArgumentNullException(nameof(recoveryCodes));

            var mergedCodes = string.Join(CodeDelimiter, recoveryCodes);
            return SetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, mergedCodes, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<bool> RedeemCodeAsync(User user, string code, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (code == null)
                throw new ArgumentNullException(nameof(code));

            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken).ConfigureAwait(false) ?? string.Empty;
            var splitCodes = mergedCodes.Split(new[] { CodeDelimiter }, StringSplitOptions.None);

            if (!splitCodes.Contains(code))
                return false;

            var updatedCodes = splitCodes.Where(s => s != code);
            await ReplaceCodesAsync(user, updatedCodes, cancellationToken).ConfigureAwait(false);

            return true;
        }

        /// <inheritdoc/>
        public virtual async Task<int> CountCodesAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken).ConfigureAwait(false) ?? string.Empty;
            return mergedCodes.Length > 0 ? mergedCodes.Split(new[] { CodeDelimiter }, StringSplitOptions.None).Length : 0;
        }

        #endregion
    }
}