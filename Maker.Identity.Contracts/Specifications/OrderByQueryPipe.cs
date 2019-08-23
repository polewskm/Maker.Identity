using System;
using System.Linq;
using System.Linq.Expressions;

namespace Maker.Identity.Contracts.Specifications
{
    public class OrderByQueryPipe<TEntity, TProperty> : IQueryPipe<TEntity>
    {
        private readonly Expression<Func<TEntity, TProperty>> _expression;
        private readonly bool _descending;

        public OrderByQueryPipe(Expression<Func<TEntity, TProperty>> expression, bool @descending)
        {
            _expression = expression ?? throw new ArgumentNullException(nameof(expression));
            _descending = @descending;
        }

        public virtual IQueryable<TEntity> Query(IQueryable<TEntity> queryRoot)
        {
            if (queryRoot is IOrderedQueryable<TEntity> orderedQueryable)
            {
                return _descending ? orderedQueryable.ThenByDescending(_expression) : orderedQueryable.ThenBy(_expression);
            }

            return _descending ? queryRoot.OrderByDescending(_expression) : queryRoot.OrderBy(_expression);
        }

    }
}