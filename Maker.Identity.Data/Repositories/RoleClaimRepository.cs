using Maker.Identity.Contracts.Entities;
using Maker.Identity.Contracts.Repositories;
using Maker.Identity.Contracts.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Data.Repositories
{
    public class RoleClaimRepository<TContext> : RepositoryEntityFramework<TContext, RoleClaim>, IRoleClaimRepository
        where TContext : DbContext
    {
        public RoleClaimRepository(TContext context, IQueryBuilder queryBuilder)
            : base(context, queryBuilder)
        {
            // nothing
        }

    }
}