using System.Linq;

namespace Maker.Identity.Contracts.Specifications
{
    public class PageQueryPipe<TEntity> : IQueryPipe<TEntity>
    {
        private readonly int _skip;
        private readonly int _take;

        public PageQueryPipe(int skip, int take)
        {
            _skip = skip;
            _take = take;
        }

        public virtual IQueryable<TEntity> Query(IQueryable<TEntity> queryRoot)
        {
            return queryRoot.Skip(_skip).Take(_take);
        }

    }
}