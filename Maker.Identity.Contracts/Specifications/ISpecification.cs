using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Maker.Identity.Contracts.Specifications
{
    public interface ISpecification<TEntity>
    {
        bool DisableTracking { get; }

        Expression<Func<TEntity, bool>> Criteria { get; }

        IReadOnlyList<IOrderBySpecification<TEntity>> OrderBySpecifications { get; }

        bool IsPagingEnabled { get; }

        int Skip { get; }

        int Take { get; }
    }

    public interface ISpecification<TIn, TOut> : ISpecification<TIn>
    {
        Expression<Func<TIn, TOut>> Projection { get; }
    }
}