using Maker.Identity.Contracts.Entities;
using Maker.Identity.Contracts.Repositories;
using Maker.Identity.Contracts.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Data.Repositories
{
    public class UserClaimRepository<TContext> : RepositoryEntityFramework<TContext, UserClaim>, IUserClaimRepository
        where TContext : DbContext
    {
        public UserClaimRepository(TContext context, IQueryBuilder queryBuilder)
            : base(context, queryBuilder)
        {
            // nothing
        }

    }
}