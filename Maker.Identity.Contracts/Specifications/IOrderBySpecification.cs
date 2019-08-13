using System.Linq;

namespace Maker.Identity.Contracts.Specifications
{
    public interface IOrderBySpecification<TEntity>
    {
        IQueryable<TEntity> Apply(IQueryable<TEntity> query);
    }
}