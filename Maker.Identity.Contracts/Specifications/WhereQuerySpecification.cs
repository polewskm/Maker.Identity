using System;
using System.Linq.Expressions;

namespace Maker.Identity.Contracts.Specifications
{
    public class WhereQuerySpecification<TEntity> : IQuerySpecification<TEntity>
    {
        public string Name => QueryNames.Where;

        public Expression<Func<TEntity, bool>> Expression { get; }

        public WhereQuerySpecification(Expression<Func<TEntity, bool>> expression)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

    }
}