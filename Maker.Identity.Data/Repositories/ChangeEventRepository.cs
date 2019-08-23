using Maker.Identity.Contracts.Events;
using Maker.Identity.Contracts.Repositories;
using Maker.Identity.Contracts.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Data.Repositories
{
    public class ChangeEventRepository<TContext> : RepositoryEntityFramework<TContext, ChangeEvent>, IChangeEventRepository
        where TContext : DbContext
    {
        public ChangeEventRepository(TContext context, IQueryBuilder queryBuilder)
            : base(context, queryBuilder)
        {
            // nothing
        }

    }
}