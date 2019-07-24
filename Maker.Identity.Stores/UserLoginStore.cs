using Maker.Identity.Stores.Entities;
using Maker.Identity.Stores.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Stores
{
    public class UserLoginStore<TContext> : StoreBase<TContext, UserLogin>
        where TContext : DbContext
    {
        public UserLoginStore(TContext context, IdentityErrorDescriber describer = null)
            : base(context, describer)
        {
            AutoSaveChanges = false;
        }

    }
}