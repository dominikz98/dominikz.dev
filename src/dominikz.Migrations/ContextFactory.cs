using dominikz.Infrastructure.Provider.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace dominikz.Migrations;

public class ContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlite("Data Source=dominikz.db", b => b.MigrationsAssembly("dominikz.Migrations"))
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .Options;

        return new DatabaseContext(options);
    }
}
