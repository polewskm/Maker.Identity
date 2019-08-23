using System.Linq;

namespace Maker.Identity.Contracts.Specifications
{
    public interface IQueryPipe<TEntity>
    {
        IQueryable<TEntity> Query(IQueryable<TEntity> queryRoot);
    }
}