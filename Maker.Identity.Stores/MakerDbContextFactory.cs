using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Maker.Identity.Stores
{
	public class MakerDbContextFactory : IDesignTimeDbContextFactory<MakerDbContext>
	{
		public MakerDbContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<MakerDbContext>();
			optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Maker.Identity;Trusted_Connection=True;MultipleActiveResultSets=true");

			return new MakerDbContext(optionsBuilder.Options);
		}

	}
}