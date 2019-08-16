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
        private readonly IDictionary<Type, IRepository> _repositories;

        public TContext Context { get; }

        public UnitOfWork(TContext context, IEnumerable<IRepositoryContext<TContext>> repositories)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));

            _repositories = repositories.ToDictionary(repository => repository.EntityType, repository => (IRepository)repository);
        }

        public virtual async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            IRepository<TEntity> repository = null;
            if (_repositories.TryGetValue(typeof(TEntity), out var repositoryBase))
            {
                repository = repositoryBase as IRepository<TEntity>;
            }

            if (repository == null)
            {
                throw new InvalidOperationException("TODO");
            }

            return repository;
        }

    }
}