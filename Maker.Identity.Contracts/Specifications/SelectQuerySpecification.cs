using System;
using System.Linq.Expressions;

namespace Maker.Identity.Contracts.Specifications
{
    public class SelectQuerySpecification<TIn, TOut> : IQuerySpecification<TIn, TOut>
    {
        public string Name => QueryNames.Select;

        public Expression<Func<TIn, TOut>> Expression { get; }

        public SelectQuerySpecification(Expression<Func<TIn, TOut>> expression)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

    }
}