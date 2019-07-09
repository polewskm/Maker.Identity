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
        private static readonly Func<UserClaim, Expression<Func<UserClaimHistory, bool>>> RetirePredicateFactory =
            roleClaim => history => history.UserClaimId == roleClaim.UserClaimId && history.RetiredWhen == Constants.MaxDateTimeOffset;

        public UserClaimStore(TContext context, IdentityErrorDescriber describer = null)
            : base(context, RetirePredicateFactory, describer)
        {
            AutoSaveChanges = false;
        }

    }
}