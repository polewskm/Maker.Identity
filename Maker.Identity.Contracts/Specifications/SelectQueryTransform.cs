using System;
using System.Linq;
using System.Linq.Expressions;

namespace Maker.Identity.Contracts.Specifications
{
    public class SelectQueryTransform<TIn, TOut> : IQueryTransform<TIn, TOut>
    {
        private readonly Expression<Func<TIn, TOut>> _expression;

        public SelectQueryTransform(Expression<Func<TIn, TOut>> expression)
        {
            _expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public virtual IQueryable<TOut> Query(IQueryable<TIn> queryRoot)
        {
            return queryRoot.Select(_expression);
        }

    }
}