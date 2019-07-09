using System;
using Microsoft.AspNetCore.Identity;

namespace Maker.Identity.Stores.Entities
{
    public interface IUserBase
    {
        long UserId { get; set; }

        string FirstName { get; set; }

        string LastName { get; set; }

        string UserName { get; set; }

        string NormalizedUserName { get; set; }

        string Email { get; set; }

        string NormalizedEmail { get; set; }

        bool EmailConfirmed { get; set; }

        string PasswordHash { get; set; }

        string SecurityStamp { get; set; }

        string PhoneNumber { get; set; }

        bool PhoneNumberConfirmed { get; set; }

        bool TwoFactorEnabled { get; set; }

        DateTimeOffset? LockoutEnd { get; set; }

        bool LockoutEnabled { get; set; }

        int AccessFailedCount { get; set; }

        DateTimeOffset MembershipCreatedWhen { get; set; }

        DateTimeOffset MembershipExpiresWhen { get; set; }
    }

    public abstract class UserBase : UserBase<UserBase>
    {
        // nothing
    }

    public abstract class UserBase<TBase> : IUserBase, ISupportAssign<TBase>
        where TBase : UserBase<TBase>
    {
        /// <summary>
        /// Gets or sets the primary key for this user.
        /// </summary>
        [PersonalData]
        public long UserId { get; set; }

        [ProtectedPersonalData]
        public string FirstName { get; set; }

        [ProtectedPersonalData]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the user name for this user.
        /// </summary>
        [ProtectedPersonalData]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the normalized user name for this user.
        /// </summary>
        public string NormalizedUserName { get; set; }

        /// <summary>
        /// Gets or sets the email address for this user.
        /// </summary>
        [ProtectedPersonalData]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the normalized email address for this user.
        /// </summary>
        public string NormalizedEmail { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if a user has confirmed their email address.
        /// </summary>
        /// <value>True if the email address has been confirmed, otherwise false.</value>
        [PersonalData]
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Gets or sets a salted and hashed representation of the password for this user.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// A random value that must change whenever a users credentials change (password changed, login removed)
        /// </summary>
        public string SecurityStamp { get; set; }

        /// <summary>
        /// Gets or sets a telephone number for the user.
        /// </summary>
        [ProtectedPersonalData]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if a user has confirmed their telephone address.
        /// </summary>
        /// <value>True if the telephone number has been confirmed, otherwise false.</value>
        [PersonalData]
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if two factor authentication is enabled for this user.
        /// </summary>
        /// <value>True if 2fa is enabled, otherwise false.</value>
        [PersonalData]
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when any user lockout ends.
        /// </summary>
        /// <remarks>
        /// A value in the past means the user is not locked out.
        /// </remarks>
        public DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if the user could be locked out.
        /// </summary>
        /// <value>True if the user could be locked out, otherwise false.</value>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// Gets or sets the number of failed login attempts for the current user.
        /// </summary>
        public int AccessFailedCount { get; set; }

        public DateTimeOffset MembershipCreatedWhen { get; set; }

        public DateTimeOffset MembershipExpiresWhen { get; set; }

        /// <summary>
        /// Returns the username for this user.
        /// </summary>
        public override string ToString() => UserName;

        public virtual void Assign(TBase other)
        {
            UserId = other.UserId;
            FirstName = other.FirstName;
            LastName = other.LastName;
            UserName = other.UserName;
            NormalizedUserName = other.NormalizedUserName;
            Email = other.Email;
            NormalizedEmail = other.NormalizedEmail;
            EmailConfirmed = other.EmailConfirmed;
            PasswordHash = other.PasswordHash;
            SecurityStamp = other.SecurityStamp;
            PhoneNumber = other.PhoneNumber;
            PhoneNumberConfirmed = other.PhoneNumberConfirmed;
            TwoFactorEnabled = other.TwoFactorEnabled;
            LockoutEnd = other.LockoutEnd;
            LockoutEnabled = other.LockoutEnabled;
            AccessFailedCount = other.AccessFailedCount;

            MembershipCreatedWhen = other.MembershipCreatedWhen;
            MembershipExpiresWhen = other.MembershipExpiresWhen;
        }
    }

    public class User : UserBase, ISupportConcurrencyToken
    {
        /// <inheritdoc/>
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    }

    public class UserHistory : UserBase, IHistoryEntity<UserBase>
    {
        /// <inheritdoc/>
        public long TransactionId { get; set; }

        /// <inheritdoc/>
        public DateTimeOffset CreatedWhen { get; set; }

        /// <inheritdoc/>
        public DateTimeOffset RetiredWhen { get; set; }
    }
}