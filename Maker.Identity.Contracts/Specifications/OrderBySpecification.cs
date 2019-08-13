using System;
using System.Linq;
using System.Linq.Expressions;

namespace Maker.Identity.Contracts.Specifications
{
    public class OrderBySpecification<TEntity, TProperty> : IOrderBySpecification<TEntity>
    {
        public bool Descending { get; set; }

        public Expression<Func<TEntity, TProperty>> Expression { get; set; }

        public virtual IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            if (query is IOrderedQueryable<TEntity> orderedQueryable)
            {
                return Descending ? orderedQueryable.ThenByDescending(Expression) : orderedQueryable.ThenBy(Expression);
            }

            return Descending ? query.OrderByDescending(Expression) : query.OrderBy(Expression);
        }

    }
}