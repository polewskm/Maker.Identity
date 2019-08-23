using System;
using System.Linq.Expressions;

namespace Maker.Identity.Contracts.Specifications
{
    public static class QueryConfiguratorExtensions
    {
        public static IQueryConfigurator<TEntity> Where<TEntity>(this IQueryConfigurator<TEntity> configurator, Expression<Func<TEntity, bool>> expression)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var specification = new WhereQuerySpecification<TEntity>(expression);
            configurator.AddSpecification(specification);

            return configurator;
        }

        public static IQueryConfigurator<TEntity> OrderBy<TEntity, TProperty>(this IQueryConfigurator<TEntity> configurator, Expression<Func<TEntity, TProperty>> expression, bool descending = false)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var specification = new OrderByQuerySpecification<TEntity, TProperty>(expression, @descending);
            configurator.AddSpecification(specification);

            return configurator;
        }

        public static IQueryConfigurator<TEntity> OrderByDescending<TEntity, TProperty>(this IQueryConfigurator<TEntity> configurator, Expression<Func<TEntity, TProperty>> expression)
        {
            return OrderBy(configurator, expression, true);
        }

        public static IQueryConfigurator<TEntity> Page<TEntity>(this IQueryConfigurator<TEntity> configurator, int skip, int take)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new PageQuerySpecification<TEntity>(skip, take);
            configurator.AddSpecification(specification);

            return configurator;
        }

        public static IQueryConfigurator<TEntity> Distinct<TEntity>(this IQueryConfigurator<TEntity> configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new DistinctQuerySpecification<TEntity>();
            configurator.AddSpecification(specification);

            return configurator;
        }

        public static IQueryConfigurator<TIn, TOut> Select<TIn, TOut>(this IQueryConfigurator<TIn> configurator, Expression<Func<TIn, TOut>> expression)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var transformSpecification = new SelectQuerySpecification<TIn, TOut>(expression);

            var inputSpecifications = configurator.OutputConfiguration.Specifications;

            return new SelectQueryConfigurator<TIn, TOut>(transformSpecification, inputSpecifications);
        }

        public static IQueryConfigurator<TIn, TOut> Output<TIn, TOut>(this IQueryConfigurator<TIn, TOut> configurator, Action<IQueryConfigurator<TOut>> callback)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(configurator);

            return configurator;
        }

    }
}