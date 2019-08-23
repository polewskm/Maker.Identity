using System;
using System.Linq.Expressions;

namespace Maker.Identity.Contracts.Specifications
{
    public interface IOrderByQuerySpecification<TEntity> : IQuerySpecification<TEntity>
    {
        Type PropertyType { get; }
    }

    public class OrderByQuerySpecification<TEntity, TProperty> : IOrderByQuerySpecification<TEntity>
    {
        public string Name => QueryNames.OrderBy;

        public Type PropertyType => typeof(TProperty);

        public Expression<Func<TEntity, TProperty>> Expression { get; }

        public bool Descending { get; }

        public OrderByQuerySpecification(Expression<Func<TEntity, TProperty>> expression, bool @descending)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            Descending = @descending;
        }

    }
}