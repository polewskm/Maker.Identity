using System;
using System.Linq;
using System.Linq.Expressions;

namespace Maker.Identity.Contracts.Specifications
{
    public class WhereQueryPipe<TEntity> : IQueryPipe<TEntity>
    {
        private readonly Expression<Func<TEntity, bool>> _expression;

        public WhereQueryPipe(Expression<Func<TEntity, bool>> expression)
        {
            _expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public virtual IQueryable<TEntity> Query(IQueryable<TEntity> queryRoot)
        {
            return queryRoot.Where(_expression);
        }

    }
}