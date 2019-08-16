using System.Threading;
using System.Threading.Tasks;
using Maker.Identity.Contracts.Repositories;

namespace Maker.Identity.Contracts
{
    public interface IUnitOfWork
    {
        Task CommitAsync(CancellationToken cancellationToken = default);

        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
    }
}