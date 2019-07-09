using IdGen;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Maker.Identity.Stores
{
    public class IdValueGenerator : ValueGenerator<long>
    {
        private readonly IIdGenerator<long> _generator;

        public IdValueGenerator(IIdGenerator<long> generator)
        {
            _generator = generator;
        }

        public override bool GeneratesTemporaryValues => false;

        public override long Next(EntityEntry entry)
        {
            return _generator.CreateId();
        }

    }
}