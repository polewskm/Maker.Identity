using System;
using System.Collections.Generic;
using System.Linq;

namespace Maker.Identity.Contracts.Specifications
{
    public class SelectQueryConfigurator<TIn, TOut> : IQueryConfigurator<TIn, TOut>, IQueryConfiguration<TIn, TOut>, IQueryConfiguration<TOut>
    {
        private readonly List<IQuerySpecification<TOut>> _outputSpecifications = new List<IQuerySpecification<TOut>>();

        public SelectQueryConfigurator(IQuerySpecification<TIn, TOut> transformSpecification, IEnumerable<IQuerySpecification<TIn>> inputSpecifications)
        {
            if (inputSpecifications == null)
                throw new ArgumentNullException(nameof(inputSpecifications));

            TransformSpecification = transformSpecification ?? throw new ArgumentNullException(nameof(transformSpecification));
            InputSpecifications = inputSpecifications.ToList();
        }

        #region IQueryConfigurator<TOut> Members

        void IQueryConfigurator<TOut>.AddSpecification(IQuerySpecification<TOut> specification)
        {
            _outputSpecifications.Add(specification);
        }

        #endregion

        #region IQueryConfiguration<TOut> Members

        IQueryConfiguration<TOut> IQueryConfigurator<TOut>.OutputConfiguration => this;

        IReadOnlyList<IQuerySpecification<TOut>> IQueryConfiguration<TOut>.Specifications => _outputSpecifications;

        #endregion

        #region IQueryConfigurator<TIn, TOut> Members

        public IQueryConfiguration<TIn, TOut> TransformConfiguration => this;

        #endregion

        #region IQueryConfiguration<TIn, TOut> Members

        public IQuerySpecification<TIn, TOut> TransformSpecification { get; }

        public IReadOnlyList<IQuerySpecification<TIn>> InputSpecifications { get; }

        public IReadOnlyList<IQuerySpecification<TOut>> OutputSpecifications => _outputSpecifications;

        #endregion

    }
}