using System;
using System.Collections.Generic;

namespace Maker.Identity.Contracts.Specifications
{
    public interface IQueryBuilder
    {
        IQueryPipe<TEntity> Build<TEntity>(IQueryConfiguration<TEntity> configuration);

        IQueryTransform<TIn, TOut> Build<TIn, TOut>(IQueryConfiguration<TIn, TOut> configuration);
    }

    public class QueryBuilder : IQueryBuilder
    {
        private readonly ICompositeQueryFactory _compositeQueryFactory;

        public QueryBuilder(ICompositeQueryFactory compositeQueryFactory)
        {
            _compositeQueryFactory = compositeQueryFactory ?? throw new ArgumentNullException(nameof(compositeQueryFactory));
        }

        public virtual IQueryPipe<TEntity> Build<TEntity>(IQueryConfiguration<TEntity> configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            return BuildPipe(configuration.Specifications);
        }

        private IQueryPipe<TEntity> BuildPipe<TEntity>(IEnumerable<IQuerySpecification<TEntity>> specifications)
        {
            IQueryPipe<TEntity> chain = new IdentityQueryPipe<TEntity>();

            foreach (var specification in specifications)
            {
                if (_compositeQueryFactory.TryCreate(specification, out var next))
                {
                    chain = new ChainQueryPipe<TEntity>(chain, next);
                }
                else
                {
                    throw new InvalidOperationException("TODO");
                }
            }

            return chain;
        }

        public virtual IQueryTransform<TIn, TOut> Build<TIn, TOut>(IQueryConfiguration<TIn, TOut> configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (!_compositeQueryFactory.TryCreate(configuration.TransformSpecification, out var transform))
                throw new InvalidOperationException("TODO");

            var inputPipe = BuildPipe(configuration.InputSpecifications);

            var outputPipe = BuildPipe(configuration.OutputSpecifications);

            var chain = new ChainQueryTransform<TIn, TOut>(inputPipe, outputPipe, transform);

            return chain;
        }

    }
}