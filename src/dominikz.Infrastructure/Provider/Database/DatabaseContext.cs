using dominikz.Infrastructure.Provider.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Infrastructure.Provider.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public IQueryable<T> From<T>() where T : class
        => Set<T>().AsQueryable();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.UseCollation("utf8mb4_general_ci");
        builder.ApplyConfigurationsFromAssembly(typeof(AccountConfiguration).Assembly);
    }
}
