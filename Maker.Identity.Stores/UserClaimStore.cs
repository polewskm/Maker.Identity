using System;
using System.Linq.Expressions;
using Maker.Identity.Stores.Entities;
using Maker.Identity.Stores.Helpers;
using Microsoft.AspNetCore.Identity;

namespace Maker.Identity.Stores
{
    public class UserClaimStore : StoreBase<UserClaim, UserClaimBase, UserClaimHistory>
    {
        private static readonly Func<UserClaim, Expression<Func<UserClaimHistory, bool>>> RetirePredicateFactory =
            roleClaim => history => history.Id == roleClaim.Id && history.RetiredWhen == Constants.MaxDateTimeOffset;

        public UserClaimStore(MakerDbContext context, IdentityErrorDescriber describer = null)
            : base(context, RetirePredicateFactory, describer)
        {
            AutoSaveChanges = false;
        }

    }
}