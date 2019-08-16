using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Maker.Identity.Contracts;
using Maker.Identity.Contracts.Repositories;
using Maker.Identity.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Data
{
    public class UnitOfWork<TContext> : IUnitOfWork
        where TContext : DbContext
    {
        private readonly IReadOnlyDictionary<Type, IRepositoryContext<TContext>> _repositories;

        public TContext Context { get; }

        public UnitOfWork(TContext context, IEnumerable<IRepositoryContext<TContext>> repositories)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));

            // make sure the specified repositories uses the same context and are of the correct entity type
            var typeOfRepositoryOpenGeneric = typeof(IRepository<>);
            var filteredRepositories = repositories.Where(repository =>
            {
                if (!ReferenceEquals(repository.Context, context))
                    return false;

                var typeOfRepositoryClosedGeneric = typeOfRepositoryOpenGeneric.MakeGenericType(repository.EntityType);
                return typeOfRepositoryClosedGeneric.IsInstanceOfType(repository);
            });

            // ToDictionary will blow up if multiple repositories exist with the same entity type
            // so only return distinct repositories by entity type (i.e. the first occurrence)
            var uniqueRepositories = filteredRepositories
                .GroupBy(repository => repository.EntityType)
                .Select(grouping => grouping.First());

            _repositories = uniqueRepositories.ToDictionary(repository => repository.EntityType, repository => repository);
        }

        public virtual async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (!_repositories.TryGetValue(typeof(TEntity), out var repositoryBase))
            {
                throw new InvalidOperationException("TODO");
            }

            // this cast is guaranteed to succeed because of the checks in the constructor
            return (IRepository<TEntity>)repositoryBase;
        }

    }
}