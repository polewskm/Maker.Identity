using System.Linq;

namespace Maker.Identity.Contracts.Specifications
{
    public interface IQueryTransform<in TIn, out TOut>
    {
        IQueryable<TOut> Query(IQueryable<TIn> queryRoot);
    }
}