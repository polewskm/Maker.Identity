using Maker.Identity.Contracts.Entities;
using Maker.Identity.Contracts.Repositories;
using Maker.Identity.Contracts.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Data.Repositories
{
    public class UserSecretRepository<TContext> : RepositoryEntityFramework<TContext, UserSecret>, IUserSecretRepository
        where TContext : DbContext
    {
        public UserSecretRepository(TContext context, IQueryBuilder queryBuilder)
            : base(context, queryBuilder)
        {
            // nothing
        }

    }
}