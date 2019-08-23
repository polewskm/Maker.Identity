namespace Maker.Identity.Contracts.Specifications
{
    public class PageQuerySpecification<TEntity> : IQuerySpecification<TEntity>
    {
        public string Name => QueryNames.Page;

        public int Skip { get; }

        public int Take { get; }

        public PageQuerySpecification(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }

    }
}