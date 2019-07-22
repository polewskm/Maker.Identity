using IdGen;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Maker.Identity.Stores
{
    public class MakerDbContextFactory : IDesignTimeDbContextFactory<MakerDbContext>
    {
        public MakerDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MakerDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Maker.Identity;Trusted_Connection=True;MultipleActiveResultSets=True");

            var idGenerator = new IdGenerator(0);

            return new MakerDbContext(optionsBuilder.Options, idGenerator);
        }

    }
}