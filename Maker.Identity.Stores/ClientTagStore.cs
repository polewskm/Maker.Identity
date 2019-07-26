using System;
using System.Linq.Expressions;
using Maker.Identity.Stores.Entities;
using Maker.Identity.Stores.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Stores
{
    public class ClientTagStore<TContext> : TagStore<TContext, ClientTag, ClientTagBase, ClientTagHistory>
        where TContext : DbContext
    {
        private readonly long _clientId;

        private static Expression<Func<ClientTagHistory, bool>> RetirePredicateFactory(ClientTag tag) => history =>
            history.ClientId == tag.ClientId
            && history.NormalizedKey == tag.NormalizedKey
            && history.RetiredWhenUtc == Constants.MaxDateTime;

        public ClientTagStore(TContext context, IdentityErrorDescriber describer, ISystemClock systemClock, long clientId)
            : base(context, describer, systemClock, RetirePredicateFactory)
        {
            _clientId = clientId;
        }

        protected override ClientTag CreateTag()
        {
            return new ClientTag { ClientId = _clientId };
        }

    }
}