using System.Reflection;
using IdGen;
using Maker.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NCode.SystemClock;

namespace Maker.Identity
{
    public class MakerDbContextFactory : IDesignTimeDbContextFactory<MakerDbContext>
    {
        public MakerDbContext CreateDbContext(string[] args)
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var optionsBuilder = new DbContextOptionsBuilder<MakerDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Maker.Identity;Trusted_Connection=True;MultipleActiveResultSets=True", b => b.MigrationsAssembly(assemblyName));

            var idGenerator = new IdGenerator(0);
            var systemClock = new SystemClockMillisecondsAccuracy();

            return new MakerDbContext(optionsBuilder.Options, idGenerator, systemClock);
        }

    }
}