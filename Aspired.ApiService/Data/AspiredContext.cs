using Aspired.ApiService.Data.Entities;

using Microsoft.EntityFrameworkCore;

namespace Aspired.ApiService.Data;

public class AspiredContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public AspiredContext(DbContextOptions options) : base(options)
    {
    }

    protected AspiredContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // for now, letting EF figure out the schema based on entity class conventions

        base.OnModelCreating(modelBuilder);
    }
}
