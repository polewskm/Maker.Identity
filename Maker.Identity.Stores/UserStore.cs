using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class UserStore : UserStore<MakerDbContext, User, UserBase, UserHistory>
    {
        public UserStore(MakerDbContext context, IdentityErrorDescriber describer = null)
            : base(context, describer)
        {
            // nothing
        }
    }

    public class UserStore<TContext, TUser, TUserBase, TUserHistory> :
        StoreBase<TContext, TUser, TUserBase, TUserHistory>,
        IProtectedUserStore<TUser>,
        IUserRoleStore<TUser>,
        IUserLoginStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserAuthenticationTokenStore<TUser>,
        IUserAuthenticatorKeyStore<TUser>,
        IUserTwoFactorRecoveryCodeStore<TUser>

        where TContext : DbContext
        where TUser : class, TUserBase, ISupportConcurrencyToken
        where TUserBase : class, IUserBase, ISupportAssign<TUserBase>
        where TUserHistory : class, TUserBase, IHistoryEntity<TUserBase>, new()
    {
        private const string InternalLoginProvider = "[MakerIdentityUserStore]";
        private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
        private const string RecoveryCodeTokenName = "RecoveryCodes";

        private static readonly Func<TUser, Expression<Func<TUserHistory, bool>>> RetirePredicateFactory =
            user => history => history.UserId == user.UserId && history.RetiredWhen == Constants.MaxDateTimeOffset;

        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserStore"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/>.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/> used to describe store errors.</param>
        public UserStore(TContext context, IdentityErrorDescriber describer = null)
            : base(context, RetirePredicateFactory, describer)
        {
            // nothing
        }

        #region Factory Methods

        /// <summary>
        /// Called to create a new instance of a <see cref="UserClaim"/>.
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="claim">The associated claim.</param>
        /// <returns></returns>
        protected virtual UserClaim CreateUserClaim(TUser user, Claim claim)
        {
            var userClaim = new UserClaim { UserId = user.UserId };
            userClaim.InitializeFromClaim(claim);
            return userClaim;
        }

        /// <summary>
        /// Called to create a new instance of a <see cref="UserLogin"/>.
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="login">The associated login.</param>
        /// <returns></returns>
        protected virtual UserLogin CreateUserLogin(TUser user, UserLoginInfo login)
        {
            return new UserLogin
            {
                UserId = user.UserId,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName
            };
        }

        /// <summary>
        /// Called to create a new instance of a <see cref="UserToken"/>.
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="loginProvider">The associated login provider.</param>
        /// <param name="name">The name of the user token.</param>
        /// <param name="value">The value of the user token.</param>
        /// <returns></returns>
        protected virtual UserToken CreateUserToken(TUser user, string loginProvider, string name, string value)
        {
            return new UserToken
            {
                UserId = user.UserId,
                LoginProvider = loginProvider,
                Name = name,
                Value = value
            };
        }

        /// <summary>
        /// Called to create a new instance of a <see cref="UserRole"/>.
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="role">The associated role.</param>
        /// <returns></returns>
        protected virtual UserRole CreateUserRole(TUser user, Role role)
        {
            return new UserRole
            {
                UserId = user.UserId,
                RoleId = role.RoleId
            };
        }

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
            return Context.Set<Role>().SingleOrDefaultAsync(
                role => role.NormalizedName == normalizedRoleName,
                cancellationToken);
        }

        /// <summary>
        /// Return a user role for the userId and roleId if it exists.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="roleId">The role's id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user role if it exists.</returns>
        protected virtual Task<UserRole> FindUserRoleAsync(string userId, string roleId, CancellationToken cancellationToken)
        {
            return Context.Set<UserRole>().FindAsync(new object[] { userId, roleId }, cancellationToken);
        }

        /// <summary>
        /// Find a user token if it exists.
        /// </summary>
        /// <param name="user">The token owner.</param>
        /// <param name="loginProvider">The login provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user token if it exists.</returns>
        protected virtual Task<UserToken> FindTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return Context.Set<UserToken>().FindAsync(
                new object[] { user.UserId, loginProvider, name },
                cancellationToken);
        }

        #endregion

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

        #region IUserStore Members

        public virtual Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.UserId);
        }

        public virtual Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.UserName);
        }

        public virtual Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.UserName = userName;

            return Task.CompletedTask;
        }

        public virtual Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.NormalizedUserName);
        }

        public virtual Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.NormalizedUserName = normalizedName;

            return Task.CompletedTask;
        }

        public virtual Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return base.FindByIdAsync(userId, cancellationToken);
        }

        public virtual Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            if (normalizedUserName == null)
                throw new ArgumentNullException(nameof(normalizedUserName));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Context.Set<TUser>().FirstOrDefaultAsync(user => user.NormalizedUserName == normalizedUserName, cancellationToken);
        }

        #endregion

        #region IUserRoleStore Members

        public virtual async Task AddToRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken)
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

            var store = new UserRoleStore<TContext>(Context, ErrorDescriber);

            await store.CreateAsync(newUserRole, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task RemoveFromRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(normalizedRoleName))
                throw new ArgumentNullException(nameof(normalizedRoleName));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var store = new UserRoleStore<TContext>(Context, ErrorDescriber);

            var role = await FindRoleAsync(normalizedRoleName, cancellationToken).ConfigureAwait(false);
            if (role != null)
            {
                var userRole = await FindUserRoleAsync(user.UserId, role.RoleId, cancellationToken).ConfigureAwait(false);
                if (userRole != null)
                {
                    await store.DeleteAsync(userRole, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public virtual async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var query = from userRole in Context.Set<UserRole>()
                        join role in Context.Set<Role>() on userRole.RoleId equals role.RoleId
                        where userRole.UserId == user.UserId
                        select role.Name;

            return await query.ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<bool> IsInRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(normalizedRoleName))
                throw new ArgumentNullException(nameof(normalizedRoleName));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var role = await FindRoleAsync(normalizedRoleName, cancellationToken).ConfigureAwait(false);
            if (role == null)
                return false;

            var userRole = await FindUserRoleAsync(user.UserId, role.RoleId, cancellationToken).ConfigureAwait(false);
            return userRole != null;
        }

        public virtual async Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(normalizedRoleName))
                throw new ArgumentNullException(nameof(normalizedRoleName));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var role = await FindRoleAsync(normalizedRoleName, cancellationToken).ConfigureAwait(false);
            if (role == null)
                return Array.Empty<TUser>();

            var query = from userRole in Context.Set<UserRole>()
                        join user in Context.Set<TUser>() on userRole.UserId equals user.UserId
                        where userRole.RoleId == role.RoleId
                        select user;

            return await query.ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region IUserLoginStore Members

        public virtual async Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (login == null)
                throw new ArgumentNullException(nameof(login));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var newUserLogin = CreateUserLogin(user, login);

            var store = new UserLoginStore<TContext>(Context, ErrorDescriber);

            await store.CreateAsync(newUserLogin, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var userLogin = await Context.Set<UserLogin>()
                .SingleOrDefaultAsync(login =>
                    login.UserId == user.UserId
                        && login.LoginProvider == loginProvider
                        && login.ProviderKey == providerKey,
                    cancellationToken)
                .ConfigureAwait(false);

            if (userLogin != null)
            {
                var store = new UserLoginStore<TContext>(Context, ErrorDescriber);
                await store.DeleteAsync(userLogin, cancellationToken).ConfigureAwait(false);
            }
        }

        public virtual async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return await Context.Set<UserLogin>()
                .Where(login => login.UserId == user.UserId)
                .Select(login => new UserLoginInfo(
                    login.LoginProvider,
                    login.ProviderKey,
                    login.ProviderDisplayName))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public virtual async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var query = from logins in Context.Set<UserLogin>()
                        join user in Context.Set<TUser>() on logins.UserId equals user.UserId
                        where logins.LoginProvider == loginProvider
                              && logins.ProviderKey == providerKey
                        select user;

            return await query.SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region IUserClaimStore Members

        public virtual async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return await Context.Set<UserClaim>()
                .Where(userClaim => userClaim.UserId == user.UserId)
                .Select(userClaim => userClaim.ToClaim())
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public virtual async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var store = new UserClaimStore<TContext>(Context, ErrorDescriber);

            var userClaims = claims.Select(claim => CreateUserClaim(user, claim));

            await store.CreateAsync(userClaims, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (claim == null)
                throw new ArgumentNullException(nameof(claim));
            if (newClaim == null)
                throw new ArgumentNullException(nameof(newClaim));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var matchedClaims = await Context.Set<UserClaim>()
                .Where(userClaim =>
                    userClaim.UserId == user.UserId
                    && userClaim.ClaimType == claim.Type
                    && userClaim.ClaimValue == claim.Value)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            foreach (var matchedClaim in matchedClaims)
            {
                matchedClaim.ClaimType = newClaim.Type;
                matchedClaim.ClaimValue = newClaim.Value;
            }

            var store = new UserClaimStore<TContext>(Context, ErrorDescriber);

            await store.UpdateAsync(matchedClaims, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var store = new UserClaimStore<TContext>(Context, ErrorDescriber);

            foreach (var claim in claims)
            {
                var matchedClaims = await Context.Set<UserClaim>()
                    .Where(userClaim =>
                        userClaim.UserId == user.UserId
                        && userClaim.ClaimType == claim.Type
                        && userClaim.ClaimValue == claim.Value)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                await store.DeleteAsync(matchedClaims, cancellationToken).ConfigureAwait(false);
            }
        }

        public virtual async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var query = from userClaims in Context.Set<UserClaim>()
                        join user in Context.Set<TUser>() on userClaims.UserId equals user.UserId
                        where userClaims.ClaimValue == claim.Value
                              && userClaims.ClaimType == claim.Type
                        select user;

            return await query.ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region IUserPasswordStore Members

        public virtual Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.PasswordHash = passwordHash;

            return Task.CompletedTask;
        }

        public virtual Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.PasswordHash);
        }

        public virtual Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.PasswordHash != null);
        }

        #endregion

        #region IUserSecurityStampStore Members

        public virtual Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.SecurityStamp = stamp;

            return Task.CompletedTask;
        }

        public virtual Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.SecurityStamp);
        }

        #endregion

        #region IUserEmailStore Members

        public virtual Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.Email = email;

            return Task.CompletedTask;
        }

        public virtual Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.Email);
        }

        public virtual Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.EmailConfirmed);
        }

        public virtual Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.EmailConfirmed = confirmed;

            return Task.CompletedTask;
        }

        public virtual Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.NormalizedEmail);
        }

        public virtual Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.NormalizedEmail = normalizedEmail;

            return Task.CompletedTask;
        }

        public virtual async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            if (normalizedEmail == null)
                throw new ArgumentNullException(nameof(normalizedEmail));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return await Context.Set<TUser>()
                .Where(user => user.NormalizedEmail == normalizedEmail)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        #endregion

        #region IUserLockoutStore Members

        public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.LockoutEnd);
        }

        public virtual Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.LockoutEnd = lockoutEnd;

            return Task.CompletedTask;
        }

        public virtual Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.AccessFailedCount++;

            return Task.FromResult(user.AccessFailedCount);
        }

        public virtual Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.AccessFailedCount = 0;

            return Task.CompletedTask;
        }

        public virtual Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.AccessFailedCount);
        }

        public virtual Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.LockoutEnabled);
        }

        public virtual Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
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

        public virtual Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.PhoneNumber = phoneNumber;

            return Task.CompletedTask;
        }

        public virtual Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.PhoneNumber);
        }

        public virtual Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public virtual Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.PhoneNumberConfirmed = confirmed;

            return Task.CompletedTask;
        }

        #endregion

        #region IQueryableUserStore Members

        public IQueryable<TUser> Users => Context.Set<TUser>();

        #endregion

        #region IUserTwoFactorStore Members

        public virtual Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            user.TwoFactorEnabled = enabled;

            return Task.CompletedTask;
        }

        public virtual Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.TwoFactorEnabled);
        }

        #endregion

        #region IUserAuthenticationTokenStore Members

        public virtual async Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var token = await FindTokenAsync(user, loginProvider, name, cancellationToken).ConfigureAwait(false);
            if (token == null)
            {
                var newToken = CreateUserToken(user, loginProvider, name, value);
                Context.Set<UserToken>().Add(newToken);
            }
            else
            {
                token.Value = value;
            }
        }

        public virtual async Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var token = await FindTokenAsync(user, loginProvider, name, cancellationToken).ConfigureAwait(false);
            if (token != null)
            {
                Context.Set<UserToken>().Remove(token);
            }
        }

        public virtual async Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
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

        public virtual Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken)
            => SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);

        public virtual Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken)
            => GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);

        #endregion

        #region IUserTwoFactorRecoveryCodeStore Members

        public virtual Task ReplaceCodesAsync(TUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            if (recoveryCodes == null)
                throw new ArgumentNullException(nameof(recoveryCodes));

            var mergedCodes = string.Join(";", recoveryCodes);
            return SetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, mergedCodes, cancellationToken);
        }

        public virtual async Task<bool> RedeemCodeAsync(TUser user, string code, CancellationToken cancellationToken)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));

            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken).ConfigureAwait(false) ?? string.Empty;
            var splitCodes = mergedCodes.Split(';');

            if (!splitCodes.Contains(code))
                return false;

            var updatedCodes = new List<string>(splitCodes.Where(s => s != code));
            await ReplaceCodesAsync(user, updatedCodes, cancellationToken).ConfigureAwait(false);

            return true;
        }

        public virtual async Task<int> CountCodesAsync(TUser user, CancellationToken cancellationToken)
        {
            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken).ConfigureAwait(false) ?? string.Empty;
            return mergedCodes.Length > 0 ? mergedCodes.Split(';').Length : 0;
        }

        #endregion

    }
}