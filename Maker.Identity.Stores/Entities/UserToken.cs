﻿using Microsoft.AspNetCore.Identity;

namespace Maker.Identity.Stores.Entities
{
    /// <summary>
    /// Represents an authentication token for a user.
    /// </summary>
    public class UserToken
    {
        /// <summary>
        /// Gets or sets the primary key of the user that the token belongs to.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the LoginProvider this token is from.
        /// </summary>
        public string LoginProvider { get; set; }

        /// <summary>
        /// Gets or sets the name of the token.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the token value.
        /// </summary>
        [ProtectedPersonalData]
        public string Value { get; set; }
    }
}