using Maker.Identity.Contracts.Entities;
using Maker.Identity.Contracts.Repositories;
using Maker.Identity.Data.Services;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Data.Repositories
{
    public class UserRepository<TContext> : RepositoryEntityFramework<TContext, User>, IUserRepository
        where TContext : DbContext
    {
        public UserRepository(TContext context, ISpecificationQueryBuilder specificationQueryBuilder)
            : base(context, specificationQueryBuilder)
        {
            // nothing
        }

    }
}