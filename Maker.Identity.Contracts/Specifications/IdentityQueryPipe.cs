using System.Linq;

namespace Maker.Identity.Contracts.Specifications
{
    public class IdentityQueryPipe<TEntity> : IQueryPipe<TEntity>
    {
        public virtual IQueryable<TEntity> Query(IQueryable<TEntity> queryRoot)
        {
            return queryRoot;
        }

    }
}