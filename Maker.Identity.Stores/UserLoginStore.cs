using System;
using System.Linq.Expressions;
using Maker.Identity.Stores.Entities;
using Maker.Identity.Stores.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Stores
{
    public class UserLoginStore<TContext> : StoreBase<TContext, UserLogin, UserLoginBase, UserLoginHistory>
        where TContext : DbContext
    {
        private static readonly Func<UserLogin, Expression<Func<UserLoginHistory, bool>>> RetirePredicateFactory =
            userLogin => history => history.LoginProvider == userLogin.LoginProvider && history.ProviderKey == userLogin.ProviderKey && history.RetiredWhenUtc == Constants.MaxDateTime;

        public UserLoginStore(TContext context, IdentityErrorDescriber describer = null)
            : base(context, RetirePredicateFactory, describer)
        {
            AutoSaveChanges = false;
        }

    }
}