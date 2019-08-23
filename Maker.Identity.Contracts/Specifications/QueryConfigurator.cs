using System;
using System.Collections.Generic;

namespace Maker.Identity.Contracts.Specifications
{
    public class QueryConfigurator<TEntity> : IQueryConfigurator<TEntity>, IQueryConfiguration<TEntity>
    {
        private readonly List<IQuerySpecification<TEntity>> _specifications = new List<IQuerySpecification<TEntity>>();

        public IQueryConfiguration<TEntity> OutputConfiguration => this;

        public IReadOnlyList<IQuerySpecification<TEntity>> Specifications => _specifications;

        void IQueryConfigurator<TEntity>.AddSpecification(IQuerySpecification<TEntity> specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(specification);
        }

    }
}