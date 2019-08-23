using System.Collections.Generic;

namespace Maker.Identity.Contracts.Specifications
{
    public interface IQueryConfiguration<TEntity>
    {
        IReadOnlyList<IQuerySpecification<TEntity>> Specifications { get; }
    }

    public interface IQueryConfiguration<TIn, TOut>
    {
        IQuerySpecification<TIn, TOut> TransformSpecification { get; }

        IReadOnlyList<IQuerySpecification<TIn>> InputSpecifications { get; }

        IReadOnlyList<IQuerySpecification<TOut>> OutputSpecifications { get; }
    }
}