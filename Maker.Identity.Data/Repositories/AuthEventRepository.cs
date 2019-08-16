using Maker.Identity.Contracts.Events;
using Maker.Identity.Contracts.Repositories;
using Maker.Identity.Data.Services;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Data.Repositories
{
    public class AuthEventRepository<TContext> : RepositoryEntityFramework<TContext, AuthEvent>, IAuthEventRepository
        where TContext : DbContext
    {
        public AuthEventRepository(TContext context, ISpecificationQueryBuilder specificationQueryBuilder)
            : base(context, specificationQueryBuilder)
        {
            // nothing
        }

    }
}