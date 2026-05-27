using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BusinessObjects;

#nullable disable

namespace DataAccessObjects
{
    public partial class MyStoreContext : DbContext
    {
        public MyStoreContext() { }

        public MyStoreContext(DbContextOptions<MyStoreContext> options) : base(options) { }

        public virtual DbSet<AccountMember> AccountMembers { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(GetConnectionString());
            }
        }

        string GetConnectionString()
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            return config["ConnectionStrings:MyStockDB"];
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountMember>(entity =>
            {
                entity.HasKey(e => e.MemberId);
                entity.Property(e => e.MemberId).HasColumnName("MemberID");
                entity.Property(e => e.EmailAddress).HasMaxLength(100);
                entity.Property(e => e.FullName).HasMaxLength(80);
                entity.Property(e => e.MemberPassword).HasMaxLength(80);

                entity.HasData(
                    new AccountMember
                    {
                        MemberId = "admin",
                        MemberPassword = "admin123",
                        FullName = "System Admin",
                        EmailAddress = "admin@mystore.local",
                        MemberRole = 1
                    },
                    new AccountMember
                    {
                        MemberId = "staff01",
                        MemberPassword = "staff123",
                        FullName = "Store Staff",
                        EmailAddress = "staff01@mystore.local",
                        MemberRole = 2
                    }
                );
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId);
                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
                entity.Property(e => e.CategoryName).HasMaxLength(15);

                entity.HasData(
                    new Category { CategoryId = 1, CategoryName = "Beverages" },
                    new Category { CategoryId = 2, CategoryName = "Snacks" },
                    new Category { CategoryId = 3, CategoryName = "Stationery" }
                );
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);
                entity.Property(e => e.ProductId).HasColumnName("ProductID");
                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
                entity.Property(e => e.ProductName).HasMaxLength(40);
                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasData(
                    new Product
                    {
                        ProductId = 1,
                        ProductName = "Green Tea",
                        CategoryId = 1,
                        UnitsInStock = 120,
                        UnitPrice = 1.99m
                    },
                    new Product
                    {
                        ProductId = 2,
                        ProductName = "Potato Chips",
                        CategoryId = 2,
                        UnitsInStock = 85,
                        UnitPrice = 2.49m
                    },
                    new Product
                    {
                        ProductId = 3,
                        ProductName = "Notebook A5",
                        CategoryId = 3,
                        UnitsInStock = 200,
                        UnitPrice = 3.25m
                    },
                    new Product
                    {
                        ProductId = 4,
                        ProductName = "Black Coffee",
                        CategoryId = 1,
                        UnitsInStock = 60,
                        UnitPrice = 4.10m
                    }
                );
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
