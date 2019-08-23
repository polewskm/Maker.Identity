using System;
using System.Threading;
using System.Threading.Tasks;
using Maker.Identity.Contracts.Entities;
using Maker.Identity.Contracts.Specifications;

namespace Maker.Identity.Contracts.Repositories
{
    public static class RepositoryExtensions
    {
        public static Task<TEntity> FindAsync<TEntity, TKey>(this IRepository<TEntity> repository, TKey id, CancellationToken cancellationToken = default)
            where TKey : IEquatable<TKey>
            where TEntity : class, ISupportId<TKey>
        {
            var queryConfiguration = Query<TEntity>.Build(configurator => configurator.Where(entity => entity.Id.Equals(id)));
            return repository.FindAsync(queryConfiguration, cancellationToken);
        }

        public static Task RemoveAsync<TEntity, TKey>(this IRepository<TEntity> repository, TKey id, CancellationToken cancellationToken = default)
            where TKey : IEquatable<TKey>
            where TEntity : class, ISupportId<TKey>
        {
            var queryConfiguration = Query<TEntity>.Build(configurator => configurator.Where(entity => entity.Id.Equals(id)));
            return repository.RemoveAsync(queryConfiguration, cancellationToken);
        }

    }
}