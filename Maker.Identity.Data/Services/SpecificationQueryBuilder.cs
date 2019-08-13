using System;
using System.Linq;
using Maker.Identity.Contracts.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Data.Services
{
    [Flags]
    public enum SpecificationQueryBuilderOptions
    {
        None = 0,
        IgnoreOrderBy = 1,
    }

    public interface ISpecificationQueryBuilder
    {
        IQueryable<T> BuildQuery<T>(IQueryable<T> queryRoot, ISpecification<T> specification, SpecificationQueryBuilderOptions options = SpecificationQueryBuilderOptions.None)
            where T : class;

        IQueryable<TOut> BuildQuery<TIn, TOut>(IQueryable<TIn> queryRoot, ISpecification<TIn, TOut> specification, SpecificationQueryBuilderOptions options = SpecificationQueryBuilderOptions.None)
            where TIn : class
            where TOut : class;
    }

    public class SpecificationQueryBuilder : ISpecificationQueryBuilder
    {
        public virtual IQueryable<T> BuildQuery<T>(IQueryable<T> queryRoot, ISpecification<T> specification, SpecificationQueryBuilderOptions options = SpecificationQueryBuilderOptions.None)
            where T : class
        {
            if (queryRoot == null)
                throw new ArgumentNullException(nameof(queryRoot));
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            var query = queryRoot;

            if (specification.DisableTracking)
            {
                query = query.AsNoTracking();
            }

            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            if (!options.HasFlag(SpecificationQueryBuilderOptions.IgnoreOrderBy))
            {
                query = specification.OrderBySpecifications.Aggregate(query, (current, next) => next.Apply(current));
            }

            if (specification.IsPagingEnabled)
            {
                query = query
                    .Skip(specification.Skip)
                    .Take(specification.Take);
            }

            return query;
        }

        public virtual IQueryable<TOut> BuildQuery<TIn, TOut>(IQueryable<TIn> queryRoot, ISpecification<TIn, TOut> specification, SpecificationQueryBuilderOptions options = SpecificationQueryBuilderOptions.None) where TIn : class where TOut : class
        {
            if (queryRoot == null)
                throw new ArgumentNullException(nameof(queryRoot));
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));
            if (specification.Projection == null)
                throw new InvalidOperationException("Projection must be specified.");

            var query = BuildQuery<TIn>(queryRoot, specification, options);

            var output = query.Select(specification.Projection);

            return output;
        }

    }
}