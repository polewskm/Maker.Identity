namespace Maker.Identity.Contracts.Specifications
{
    public class WhereQueryPipeFactory : IQueryPipeFactory
    {
        public string Name => QueryNames.Where;

        public virtual bool TryCreate<TEntity>(IQuerySpecification<TEntity> specification, out IQueryPipe<TEntity> queryPipe)
        {
            if (specification is WhereQuerySpecification<TEntity> whereSpec)
            {
                queryPipe = new WhereQueryPipe<TEntity>(whereSpec.Expression);
                return true;
            }

            queryPipe = null;
            return false;
        }

    }
}