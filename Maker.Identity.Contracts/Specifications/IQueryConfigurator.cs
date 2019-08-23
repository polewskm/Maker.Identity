using System.ComponentModel;

namespace Maker.Identity.Contracts.Specifications
{
    public interface IQueryConfigurator<TEntity>
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        IQueryConfiguration<TEntity> OutputConfiguration { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        void AddSpecification(IQuerySpecification<TEntity> specification);
    }

    public interface IQueryConfigurator<TIn, TOut> : IQueryConfigurator<TOut>
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        IQueryConfiguration<TIn, TOut> TransformConfiguration { get; }
    }
}