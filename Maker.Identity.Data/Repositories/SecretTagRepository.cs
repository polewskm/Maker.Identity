using Maker.Identity.Contracts.Entities;
using Maker.Identity.Contracts.Repositories;
using Maker.Identity.Contracts.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Data.Repositories
{
    public class SecretTagRepository<TContext> : RepositoryEntityFramework<TContext, SecretTag>, ISecretTagRepository
        where TContext : DbContext
    {
        public SecretTagRepository(TContext context, IQueryBuilder queryBuilder)
            : base(context, queryBuilder)
        {
            // nothing
        }

    }
}