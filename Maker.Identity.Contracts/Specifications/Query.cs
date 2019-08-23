using System;

namespace Maker.Identity.Contracts.Specifications
{
    public static class Query<TEntity>
    {
        public static IQueryConfiguration<TEntity> Build(Func<IQueryConfigurator<TEntity>, IQueryConfigurator<TEntity>> callback)
        {
            var input = new QueryConfigurator<TEntity>();

            var output = callback(input);

            return output.OutputConfiguration;
        }

        public static IQueryConfiguration<TEntity, TOut> Build<TOut>(Func<IQueryConfigurator<TEntity>, IQueryConfigurator<TEntity, TOut>> callback)
        {
            var input = new QueryConfigurator<TEntity>();

            var output = callback(input);

            return output.TransformConfiguration;
        }

    }
}