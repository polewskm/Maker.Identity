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
            var specification = new Specification<TEntity>
            {
                Criteria = entity => entity.Id.Equals(id)
            };
            return repository.FindAsync(specification, cancellationToken);
        }

        public static Task RemoveAsync<TEntity, TKey>(this IRepository<TEntity> repository, TKey id, CancellationToken cancellationToken = default)
            where TKey : IEquatable<TKey>
            where TEntity : class, ISupportId<TKey>
        {
            var specification = new Specification<TEntity>
            {
                Criteria = entity => entity.Id.Equals(id)
            };
            return repository.RemoveAsync(specification, cancellationToken);
        }

    }
}