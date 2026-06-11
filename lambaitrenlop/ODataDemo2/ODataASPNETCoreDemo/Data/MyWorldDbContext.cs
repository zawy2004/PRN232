using Microsoft.EntityFrameworkCore;
using ODataASPNETCoreDemo.Data.Entities;

namespace ODataASPNETCoreDemo.Data;

public class MyWorldDbContext : DbContext
{
    public MyWorldDbContext(DbContextOptions<MyWorldDbContext> options) : base(options)
    {
    }

    public DbSet<Gadgets> Gadgets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Gadgets>(entity =>
        {
            entity.Property(e => e.Cost).HasPrecision(18, 0);
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // below line to watch the ef core sql queries generation
        // not at all recommended for the production code
        optionsBuilder.LogTo(Console.WriteLine);
    }
}
