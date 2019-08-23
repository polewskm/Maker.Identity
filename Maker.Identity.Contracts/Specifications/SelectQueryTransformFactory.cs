namespace Maker.Identity.Contracts.Specifications
{
    public class SelectQueryTransformFactory : IQueryTransformFactory
    {
        public string Name => QueryNames.Select;

        public virtual bool TryCreate<TIn, TOut>(IQuerySpecification<TIn, TOut> specification, out IQueryTransform<TIn, TOut> transform)
        {
            if (specification is SelectQuerySpecification<TIn, TOut> selectSpec)
            {
                transform = new SelectQueryTransform<TIn, TOut>(selectSpec.Expression);
                return true;
            }

            transform = null;
            return true;
        }

    }
}