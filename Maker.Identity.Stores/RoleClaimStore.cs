﻿using System;
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
            roleClaim => history => history.RoleClaimId == roleClaim.RoleClaimId && history.RetiredWhenUtc == Constants.MaxDateTime;

        public RoleClaimStore(TContext context, IdentityErrorDescriber describer = null)
            : base(context, RetirePredicateFactory, describer)
        {
            AutoSaveChanges = false;
        }

    }
}