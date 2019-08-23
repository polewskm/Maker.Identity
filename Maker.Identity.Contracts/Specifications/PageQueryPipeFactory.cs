namespace Maker.Identity.Contracts.Specifications
{
    public class PageQueryPipeFactory : IQueryPipeFactory
    {
        public string Name => QueryNames.Page;

        public virtual bool TryCreate<TEntity>(IQuerySpecification<TEntity> specification, out IQueryPipe<TEntity> queryPipe)
        {
            if (specification is PageQuerySpecification<TEntity> pageSpec)
            {
                queryPipe = new PageQueryPipe<TEntity>(pageSpec.Skip, pageSpec.Take);
                return true;
            }

            queryPipe = null;
            return false;
        }

    }
}