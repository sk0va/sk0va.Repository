using Microsoft.EntityFrameworkCore;

namespace Sample.App;

public class CompanyDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("Company");
        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<DbPerson> People { get; set; }
}
