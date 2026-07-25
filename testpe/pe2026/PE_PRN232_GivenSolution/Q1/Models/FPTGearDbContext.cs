using Microsoft.EntityFrameworkCore;

namespace Q1.Models
{
    public class FPTGearDbContext : DbContext
    {
        public FPTGearDbContext(DbContextOptions<FPTGearDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Equipment> Equipments => Set<Equipment>();
        public DbSet<Rental> Rentals => Set<Rental>();
    }
}
