using Maker.Identity.Contracts.Entities;
using Maker.Identity.Contracts.Repositories;
using Maker.Identity.Data.Services;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Data.Repositories
{
    public class SecretRepository<TContext> : RepositoryEntityFramework<TContext, Secret>, ISecretRepository
        where TContext : DbContext
    {
        public SecretRepository(TContext context, ISpecificationQueryBuilder specificationQueryBuilder)
            : base(context, specificationQueryBuilder)
        {
            // nothing
        }

    }
}