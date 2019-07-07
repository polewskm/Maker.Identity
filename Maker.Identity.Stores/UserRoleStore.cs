using System;
using System.Linq.Expressions;
using Maker.Identity.Stores.Entities;
using Maker.Identity.Stores.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Stores
{
    public class UserRoleStore<TContext> : StoreBase<TContext, UserRole, UserRoleBase, UserRoleHistory>
        where TContext : DbContext
    {
        private static readonly Func<UserRole, Expression<Func<UserRoleHistory, bool>>> RetirePredicateFactory =
            userRole => history => history.UserId == userRole.UserId && history.RoleId == userRole.RoleId && history.RetiredWhen == Constants.MaxDateTimeOffset;

        public UserRoleStore(TContext context, IdentityErrorDescriber describer = null)
            : base(context, RetirePredicateFactory, describer)
        {
            AutoSaveChanges = false;
        }

    }
}