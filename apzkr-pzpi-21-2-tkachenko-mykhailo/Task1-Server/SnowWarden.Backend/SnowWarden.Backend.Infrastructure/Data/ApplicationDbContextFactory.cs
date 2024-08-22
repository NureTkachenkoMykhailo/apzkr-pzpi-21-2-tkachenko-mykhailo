using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SnowWarden.Backend.Infrastructure.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
	private const string CONNECTION_STRING = "";

	public ApplicationDbContext CreateDbContext(string[] args)
	{
		DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();

		optionsBuilder.UseNpgsql(CONNECTION_STRING);

		return new ApplicationDbContext(optionsBuilder.Options, null);
	}
}