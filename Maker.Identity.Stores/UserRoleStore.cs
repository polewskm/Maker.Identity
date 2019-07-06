using System;
using System.Linq.Expressions;
using Maker.Identity.Stores.Entities;
using Maker.Identity.Stores.Helpers;
using Microsoft.AspNetCore.Identity;

namespace Maker.Identity.Stores
{
    public class UserRoleStore : StoreBase<UserRole, UserRoleBase, UserRoleHistory>
    {
        private static readonly Func<UserRole, Expression<Func<UserRoleHistory, bool>>> RetirePredicateFactory =
            userRole => history => history.UserId == userRole.UserId && history.RoleId == userRole.RoleId && history.RetiredWhen == Constants.MaxDateTimeOffset;

        public UserRoleStore(MakerDbContext context, IdentityErrorDescriber describer = null)
            : base(context, RetirePredicateFactory, describer)
        {
            AutoSaveChanges = false;
        }

    }
}