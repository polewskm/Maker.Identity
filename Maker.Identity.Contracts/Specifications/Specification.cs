using System;
using System.Linq.Expressions;

namespace Maker.Identity.Contracts.Specifications
{
    public class Specification<T> : ISpecification<T>
    {
        public bool DisableTracking { get; set; }

        public Expression<Func<T, bool>> Criteria { get; set; }

        public Expression<Func<T, object>> OrderBy { get; set; }

        public bool OrderByDescending { get; set; }

        public bool EnablePaging { get; private set; }

        public int Skip { get; private set; }

        public int Take { get; private set; }

        public virtual Specification<T> SetCriteria(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;

            return this;
        }

        public virtual Specification<T> SetOrderBy(Expression<Func<T, object>> orderBy, bool descending = false)
        {
            OrderBy = orderBy;
            OrderByDescending = descending;

            return this;
        }

        public virtual Specification<T> ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            EnablePaging = true;

            return this;
        }
    }

    public class Specification<TIn, TOut> : Specification<TIn>, ISpecification<TIn, TOut>
    {
        public Expression<Func<TIn, TOut>> Projection { get; set; }
    }
}