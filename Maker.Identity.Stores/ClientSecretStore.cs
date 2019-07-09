using System;
using System.Linq.Expressions;
using Maker.Identity.Stores.Entities;
using Maker.Identity.Stores.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Stores
{
    public class ClientSecretStore<TContext> : StoreBase<TContext, ClientSecret, ClientSecretBase, ClientSecretHistory>
        where TContext : DbContext
    {
        private static readonly Func<ClientSecret, Expression<Func<ClientSecretHistory, bool>>> RetirePredicateFactory =
            client => history => history.ClientId == client.ClientId && history.SecretId == client.SecretId && history.RetiredWhen == Constants.MaxDateTimeOffset;

        public ClientSecretStore(TContext context, IdentityErrorDescriber describer = null)
            : base(context, RetirePredicateFactory, describer)
        {
            // nothing
        }

    }
}