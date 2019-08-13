using System;
using System.Linq.Expressions;

namespace Maker.Identity.Contracts.Specifications
{
    public interface ISpecification<T>
    {
        bool DisableTracking { get; }

        Expression<Func<T, bool>> Criteria { get; }

        Expression<Func<T, object>> OrderBy { get; }

        bool OrderByDescending { get; }

        bool EnablePaging { get; }

        int Skip { get; }

        int Take { get; }
    }

    public interface ISpecification<TIn, TOut> : ISpecification<TIn>
    {
        Expression<Func<TIn, TOut>> Projection { get; }
    }
}