using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Maker.Identity.Contracts.Specifications
{
    public class Specification<TEntity> : ISpecification<TEntity>
    {
        private readonly List<IOrderBySpecification<TEntity>> _orderBySpecifications = new List<IOrderBySpecification<TEntity>>();

        public bool DisableTracking { get; set; }

        public Expression<Func<TEntity, bool>> Criteria { get; set; }

        public IReadOnlyList<IOrderBySpecification<TEntity>> OrderBySpecifications => _orderBySpecifications;

        public bool IsPagingEnabled { get; private set; }

        public int Skip { get; private set; }

        public int Take { get; private set; }

        public virtual Specification<TEntity> OrderBy<TProperty>(Expression<Func<TEntity, TProperty>> expression, bool descending = false)
        {
            _orderBySpecifications.Add(new OrderBySpecification<TEntity, TProperty>
            {
                Expression = expression,
                Descending = descending,
            });

            return this;
        }

        public virtual Specification<TEntity> Page(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;

            return this;
        }
    }

    public class Specification<TIn, TOut> : Specification<TIn>, ISpecification<TIn, TOut>
    {
        public Expression<Func<TIn, TOut>> Projection { get; set; }
    }
}