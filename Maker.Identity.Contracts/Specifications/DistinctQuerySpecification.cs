namespace Maker.Identity.Contracts.Specifications
{
    public class DistinctQuerySpecification<TEntity> : IQuerySpecification<TEntity>
    {
        public string Name => QueryNames.Distinct;
    }
}