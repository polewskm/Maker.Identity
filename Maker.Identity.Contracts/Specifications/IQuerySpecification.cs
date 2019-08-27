namespace Maker.Identity.Contracts.Specifications
{
    public interface IQuerySpecification<TEntity> : IQueryName
    {
        // nothing
    }

    public interface IQuerySpecification<TIn, TOut> : IQueryName
    {
        // nothing
    }
}