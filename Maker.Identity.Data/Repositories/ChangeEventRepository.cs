using Maker.Identity.Contracts.Audit;
using Maker.Identity.Contracts.Repositories;
using Maker.Identity.Data.Services;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Data.Repositories
{
    public class ChangeEventRepository<TContext> : RepositoryEntityFramework<TContext, ChangeEvent>, IChangeEventRepository
        where TContext : DbContext
    {
        public ChangeEventRepository(TContext context, ISpecificationQueryBuilder specificationQueryBuilder)
            : base(context, specificationQueryBuilder)
        {
            // nothing
        }

    }
}