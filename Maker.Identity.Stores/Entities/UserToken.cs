using System;
using Microsoft.AspNetCore.Identity;

namespace Maker.Identity.Stores.Entities
{
    /// <summary>
    /// Represents an authentication token for a user.
    /// </summary>
    public class UserTokenBase : ISupportAssign<UserTokenBase>
    {
        /// <summary>
        /// Gets or sets the primary key of the user that the token belongs to.
        /// </summary>
        public virtual string UserId { get; set; }

        /// <summary>Gets or sets the LoginProvider this token is from.</summary>
        public virtual string LoginProvider { get; set; }

        /// <summary>Gets or sets the name of the token.</summary>
        public virtual string Name { get; set; }

        /// <summary>Gets or sets the token value.</summary>
        [ProtectedPersonalData]
        public virtual string Value { get; set; }

        public virtual void Assign(UserTokenBase other)
        {
            UserId = other.UserId;
            LoginProvider = other.LoginProvider;
            Name = other.Name;
            Value = other.Value;
        }
    }

    public class UserToken : UserTokenBase
    {
        // nothing
    }

    public class UserTokenHistory : UserTokenBase, IHistoryEntity<UserTokenBase>
    {
        /// <inheritdoc/>
        public string TransactionId { get; set; }

        /// <inheritdoc/>
        public DateTimeOffset CreatedWhen { get; set; }

        /// <inheritdoc/>
        public DateTimeOffset RetiredWhen { get; set; }
    }
}