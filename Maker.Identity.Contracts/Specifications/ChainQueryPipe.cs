using System;
using System.Linq;

namespace Maker.Identity.Contracts.Specifications
{
    public class ChainQueryPipe<TEntity> : IQueryPipe<TEntity>
    {
        private readonly IQueryPipe<TEntity> _current;
        private readonly IQueryPipe<TEntity> _next;

        public ChainQueryPipe(IQueryPipe<TEntity> current, IQueryPipe<TEntity> next)
        {
            _current = current ?? throw new ArgumentNullException(nameof(current));
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public virtual IQueryable<TEntity> Query(IQueryable<TEntity> queryRoot)
        {
            var query = _current.Query(queryRoot);

            query = _next.Query(query);

            return query;
        }
    }
}