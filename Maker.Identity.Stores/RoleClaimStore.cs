using System;
using System.Linq.Expressions;
using Maker.Identity.Stores.Entities;
using Maker.Identity.Stores.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Stores
{
    public class RoleClaimStore<TContext> : StoreBase<TContext, RoleClaim, RoleClaimBase, RoleClaimHistory>
        where TContext : DbContext
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly Func<RoleClaim, Expression<Func<RoleClaimHistory, bool>>> RetirePredicateFactory =
            roleClaim => history => history.ClaimId == roleClaim.ClaimId && history.RetiredWhenUtc == Constants.MaxDateTime;

        public RoleClaimStore(TContext context, IdentityErrorDescriber describer, ISystemClock systemClock)
            : base(context, describer, systemClock, RetirePredicateFactory)
        {
            AutoSaveChanges = false;
        }

    }
}