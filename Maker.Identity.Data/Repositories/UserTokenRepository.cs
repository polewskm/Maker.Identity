using Maker.Identity.Contracts.Entities;
using Maker.Identity.Contracts.Repositories;
using Maker.Identity.Contracts.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Data.Repositories
{
    public class UserTokenRepository<TContext> : RepositoryEntityFramework<TContext, UserToken>, IUserTokenRepository
        where TContext : DbContext
    {
        public UserTokenRepository(TContext context, IQueryBuilder queryBuilder)
            : base(context, queryBuilder)
        {
            // nothing
        }

    }
}