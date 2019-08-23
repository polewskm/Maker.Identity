using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Maker.Identity.Contracts.Specifications;

namespace Maker.Identity.Contracts.Repositories
{
    public interface IRepository
    {
        Type EntityType { get; }
    }

    public interface IReadRepository<TEntity> : IRepository
    {
        Task<TEntity> FindAsync(IQueryConfiguration<TEntity> queryConfiguration, CancellationToken cancellationToken = default);

        Task<TOut> FindAsync<TOut>(IQueryConfiguration<TEntity, TOut> queryConfiguration, CancellationToken cancellationToken = default) where TOut : class;

        Task<IReadOnlyList<TEntity>> ListAsync(IQueryConfiguration<TEntity> queryConfiguration, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TOut>> ListAsync<TOut>(IQueryConfiguration<TEntity, TOut> queryConfiguration, CancellationToken cancellationToken = default) where TOut : class;

        Task<int> CountAsync(IQueryConfiguration<TEntity> queryConfiguration, CancellationToken cancellationToken = default);
    }

    public interface IRepository<TEntity> : IReadRepository<TEntity>
        where TEntity : class
    {
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task RemoveAsync(IQueryConfiguration<TEntity> queryConfiguration, CancellationToken cancellationToken = default);
    }
}