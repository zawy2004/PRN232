using FUNewsManagementSystem.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.DataAccess;

public class FunewsDbContext : DbContext
{
    public FunewsDbContext(DbContextOptions<FunewsDbContext> options)
        : base(options)
    {
    }

    public DbSet<SystemAccount> SystemAccounts => Set<SystemAccount>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<NewsArticle> NewsArticles => Set<NewsArticle>();

    public DbSet<Tag> Tags => Set<Tag>();

    public DbSet<NewsTag> NewsTags => Set<NewsTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NewsTag>()
            .HasKey(nt => new { nt.NewsArticleId, nt.TagId });

        modelBuilder.Entity<NewsTag>()
            .HasOne(nt => nt.NewsArticle)
            .WithMany(na => na.NewsTags)
            .HasForeignKey(nt => nt.NewsArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NewsTag>()
            .HasOne(nt => nt.Tag)
            .WithMany(t => t.NewsTags)
            .HasForeignKey(nt => nt.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NewsArticle>()
            .HasOne(na => na.Category)
            .WithMany(c => c.NewsArticles)
            .HasForeignKey(na => na.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NewsArticle>()
            .HasOne(na => na.CreatedBy)
            .WithMany(a => a.CreatedNewsArticles)
            .HasForeignKey(na => na.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NewsArticle>()
            .HasOne(na => na.UpdatedBy)
            .WithMany(a => a.UpdatedNewsArticles)
            .HasForeignKey(na => na.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}
