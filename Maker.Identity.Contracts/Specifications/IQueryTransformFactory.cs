namespace Maker.Identity.Contracts.Specifications
{
    public interface IQueryTransformFactory : IQueryName
    {
        bool TryCreate<TIn, TOut>(IQuerySpecification<TIn, TOut> specification, out IQueryTransform<TIn, TOut> transform);
    }
}