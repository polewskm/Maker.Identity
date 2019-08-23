namespace Maker.Identity.Contracts.Specifications
{
    public interface IQueryPipeFactory : IQueryName
    {
        bool TryCreate<TEntity>(IQuerySpecification<TEntity> specification, out IQueryPipe<TEntity> queryPipe);
    }
}