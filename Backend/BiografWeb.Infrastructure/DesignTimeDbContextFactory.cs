using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BiografWeb.Infrastructure;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        // Fallback connection string for migrations at design-time
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=bio_db;Username=bio;Password=bio_pw");
        return new AppDbContext(optionsBuilder.Options);
    }
}


