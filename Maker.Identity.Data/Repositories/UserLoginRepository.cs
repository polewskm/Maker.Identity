using Maker.Identity.Contracts.Entities;
using Maker.Identity.Contracts.Repositories;
using Maker.Identity.Contracts.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Data.Repositories
{
    public class UserLoginRepository<TContext> : RepositoryEntityFramework<TContext, UserLogin>, IUserLoginRepository
        where TContext : DbContext
    {
        public UserLoginRepository(TContext context, IQueryBuilder queryBuilder)
            : base(context, queryBuilder)
        {
            // nothing
        }

    }
}