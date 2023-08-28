using Microsoft.EntityFrameworkCore;

namespace Sample.App;

public class CompanyDbContext : DbContext
{
    public CompanyDbContext(DbContextOptions<CompanyDbContext> options) : base(options)
    {
    }

    public DbSet<DbPerson> People { get; set; }
    public DbSet<DbJob> Jobs { get; set; }
}
