using System;
using System.Linq.Expressions;
using Maker.Identity.Stores.Entities;
using Maker.Identity.Stores.Helpers;
using Microsoft.AspNetCore.Identity;

namespace Maker.Identity.Stores
{
    public class RoleClaimStore : StoreBase<RoleClaim, RoleClaimBase, RoleClaimHistory>
    {
        private static readonly Func<RoleClaim, Expression<Func<RoleClaimHistory, bool>>> RetirePredicateFactory =
            roleClaim => history => history.Id == roleClaim.Id && history.RetiredWhen == Constants.MaxDateTimeOffset;

        public RoleClaimStore(MakerDbContext context, IdentityErrorDescriber describer = null)
            : base(context, RetirePredicateFactory, describer)
        {
            AutoSaveChanges = false;
        }

    }
}