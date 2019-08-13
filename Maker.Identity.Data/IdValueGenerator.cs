using System;
using IdGen;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Maker.Identity.Data
{
    public class IdValueGenerator : ValueGenerator<long>
    {
        private readonly IIdGenerator<long> _generator;

        public IdValueGenerator(IIdGenerator<long> generator)
        {
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        }

        public override bool GeneratesTemporaryValues => false;

        public override long Next(EntityEntry entry)
        {
            return _generator.CreateId();
        }

    }
}