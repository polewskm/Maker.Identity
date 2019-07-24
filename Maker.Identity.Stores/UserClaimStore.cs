using System;
using System.Linq.Expressions;
using Maker.Identity.Stores.Entities;
using Maker.Identity.Stores.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Stores
{
    public class UserClaimStore<TContext> : StoreBase<TContext, UserClaim, UserClaimBase, UserClaimHistory>
        where TContext : DbContext
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly Func<UserClaim, Expression<Func<UserClaimHistory, bool>>> RetirePredicateFactory =
            roleClaim => history => history.UserClaimId == roleClaim.UserClaimId && history.RetiredWhenUtc == Constants.MaxDateTime;

        public UserClaimStore(TContext context, IdentityErrorDescriber describer, ISystemClock systemClock)
            : base(context, describer, systemClock, RetirePredicateFactory)
        {
            AutoSaveChanges = false;
        }

    }
}