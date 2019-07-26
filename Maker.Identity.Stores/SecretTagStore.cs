using System;
using System.Linq.Expressions;
using Maker.Identity.Stores.Entities;
using Maker.Identity.Stores.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Stores
{
    public class SecretTagStore<TContext> : TagStore<TContext, SecretTag, SecretTagBase, SecretTagHistory>
        where TContext : DbContext
    {
        private readonly long _secretId;

        private static Expression<Func<SecretTagHistory, bool>> RetirePredicateFactory(SecretTag tag) => history =>
            history.SecretId == tag.SecretId
            && history.NormalizedKey == tag.NormalizedKey
            && history.RetiredWhenUtc == Constants.MaxDateTime;

        public SecretTagStore(TContext context, IdentityErrorDescriber describer, ISystemClock systemClock, long secretId)
            : base(context, describer, systemClock, RetirePredicateFactory)
        {
            _secretId = secretId;
        }

        protected override SecretTag CreateTag()
        {
            return new SecretTag { SecretId = _secretId };
        }

    }
}