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
        private static readonly Func<RoleClaim, Expression<Func<RoleClaimHistory, bool>>> RetirePredicateFactory =
            roleClaim => history => history.Id == roleClaim.Id && history.RetiredWhen == Constants.MaxDateTimeOffset;

        public RoleClaimStore(TContext context, IdentityErrorDescriber describer = null)
            : base(context, RetirePredicateFactory, describer)
        {
            AutoSaveChanges = false;
        }

    }
}