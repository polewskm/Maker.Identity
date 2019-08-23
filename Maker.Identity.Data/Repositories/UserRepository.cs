using Maker.Identity.Contracts.Entities;
using Maker.Identity.Contracts.Repositories;
using Maker.Identity.Contracts.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Data.Repositories
{
    public class UserRepository<TContext> : RepositoryEntityFramework<TContext, User>, IUserRepository
        where TContext : DbContext
    {
        public UserRepository(TContext context, IQueryBuilder queryBuilder)
            : base(context, queryBuilder)
        {
            // nothing
        }

    }
}