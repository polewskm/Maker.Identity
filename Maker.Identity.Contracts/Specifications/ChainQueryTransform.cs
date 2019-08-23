using System;
using System.Linq;

namespace Maker.Identity.Contracts.Specifications
{
    public class ChainQueryTransform<TIn, TOut> : IQueryTransform<TIn, TOut>
    {
        private readonly IQueryPipe<TIn> _inputPipe;
        private readonly IQueryPipe<TOut> _outputPipe;
        private readonly IQueryTransform<TIn, TOut> _transform;

        public ChainQueryTransform(IQueryPipe<TIn> inputPipe, IQueryPipe<TOut> outputPipe, IQueryTransform<TIn, TOut> transform)
        {
            _inputPipe = inputPipe ?? throw new ArgumentNullException(nameof(inputPipe));
            _outputPipe = outputPipe ?? throw new ArgumentNullException(nameof(outputPipe));
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
        }

        public virtual IQueryable<TOut> Query(IQueryable<TIn> queryRoot)
        {
            var inputQuery = _inputPipe.Query(queryRoot);

            var transformQuery = _transform.Query(inputQuery);

            var outputQuery = _outputPipe.Query(transformQuery);

            return outputQuery;
        }

    }
}