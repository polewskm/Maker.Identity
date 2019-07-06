using System;
using Microsoft.AspNetCore.Identity;

namespace Maker.Identity.Stores.Entities
{
	public abstract class UserBase : ISupportAssign<UserBase>
	{
		/// <summary>
		/// Gets or sets the primary key for this user.
		/// </summary>
		[PersonalData]
		public virtual string UserId { get; set; } = Guid.NewGuid().ToString();

		[ProtectedPersonalData]
		public string FirstName { get; set; }

		[ProtectedPersonalData]
		public string LastName { get; set; }

		/// <summary>
		/// Gets or sets the user name for this user.
		/// </summary>
		[ProtectedPersonalData]
		public virtual string UserName { get; set; }

		/// <summary>
		/// Gets or sets the normalized user name for this user.
		/// </summary>
		public virtual string NormalizedUserName { get; set; }

		/// <summary>
		/// Gets or sets the email address for this user.
		/// </summary>
		[ProtectedPersonalData]
		public virtual string Email { get; set; }

		/// <summary>
		/// Gets or sets the normalized email address for this user.
		/// </summary>
		public virtual string NormalizedEmail { get; set; }

		/// <summary>
		/// Gets or sets a flag indicating if a user has confirmed their email address.
		/// </summary>
		/// <value>True if the email address has been confirmed, otherwise false.</value>
		[PersonalData]
		public virtual bool EmailConfirmed { get; set; }

		/// <summary>
		/// Gets or sets a salted and hashed representation of the password for this user.
		/// </summary>
		public virtual string PasswordHash { get; set; }

		/// <summary>
		/// A random value that must change whenever a users credentials change (password changed, login removed)
		/// </summary>
		public virtual string SecurityStamp { get; set; }

		/// <summary>
		/// Gets or sets a telephone number for the user.
		/// </summary>
		[ProtectedPersonalData]
		public virtual string PhoneNumber { get; set; }

		/// <summary>
		/// Gets or sets a flag indicating if a user has confirmed their telephone address.
		/// </summary>
		/// <value>True if the telephone number has been confirmed, otherwise false.</value>
		[PersonalData]
		public virtual bool PhoneNumberConfirmed { get; set; }

		/// <summary>
		/// Gets or sets a flag indicating if two factor authentication is enabled for this user.
		/// </summary>
		/// <value>True if 2fa is enabled, otherwise false.</value>
		[PersonalData]
		public virtual bool TwoFactorEnabled { get; set; }

		/// <summary>
		/// Gets or sets the date and time, in UTC, when any user lockout ends.
		/// </summary>
		/// <remarks>
		/// A value in the past means the user is not locked out.
		/// </remarks>
		public virtual DateTimeOffset? LockoutEnd { get; set; }

		/// <summary>
		/// Gets or sets a flag indicating if the user could be locked out.
		/// </summary>
		/// <value>True if the user could be locked out, otherwise false.</value>
		public virtual bool LockoutEnabled { get; set; }

		/// <summary>
		/// Gets or sets the number of failed login attempts for the current user.
		/// </summary>
		public virtual int AccessFailedCount { get; set; }

		public DateTimeOffset MembershipCreatedWhen { get; set; }

		public DateTimeOffset MembershipExpiresWhen { get; set; }

		/// <summary>
		/// Returns the username for this user.
		/// </summary>
		public override string ToString() => UserName;

		public virtual void Assign(UserBase other)
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
		public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
	}

	public class UserHistory : UserBase, IHistoryEntity<UserBase>
	{
		/// <inheritdoc/>
		public string TransactionId { get; set; } = Guid.NewGuid().ToString();

		/// <inheritdoc/>
		public DateTimeOffset CreatedWhen { get; set; }

		/// <inheritdoc/>
		public DateTimeOffset RetiredWhen { get; set; }
	}
}