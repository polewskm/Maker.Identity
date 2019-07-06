using System;
using System.Linq.Expressions;
using Maker.Identity.Stores.Entities;
using Maker.Identity.Stores.Helpers;
using Microsoft.AspNetCore.Identity;

namespace Maker.Identity.Stores
{
    public class UserLoginStore : StoreBase<UserLogin, UserLoginBase, UserLoginHistory>
    {
        private static readonly Func<UserLogin, Expression<Func<UserLoginHistory, bool>>> RetirePredicateFactory =
            userLogin => history => history.LoginProvider == userLogin.LoginProvider && history.ProviderKey == userLogin.ProviderKey && history.RetiredWhen == Constants.MaxDateTimeOffset;

        public UserLoginStore(MakerDbContext context, IdentityErrorDescriber describer = null)
            : base(context, RetirePredicateFactory, describer)
        {
            AutoSaveChanges = false;
        }

    }
}