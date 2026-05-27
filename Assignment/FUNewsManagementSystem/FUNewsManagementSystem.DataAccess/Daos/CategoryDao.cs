using FUNewsManagementSystem.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.DataAccess.Daos;

public sealed class CategoryDao
{
    private static readonly Lazy<CategoryDao> LazyInstance = new(() => new CategoryDao());

    public static CategoryDao Instance => LazyInstance.Value;

    private CategoryDao()
    {
    }

    public async Task<List<Category>> GetAllAsync()
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        return await context.Categories.AsNoTracking().ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        return await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.CategoryId == id);
    }

    public async Task<Category> AddAsync(Category category)
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task<bool> UpdateAsync(Category category)
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        var existing = await context.Categories.FirstOrDefaultAsync(c => c.CategoryId == category.CategoryId);
        if (existing is null)
        {
            return false;
        }

        existing.CategoryName = category.CategoryName;
        existing.CategoryDescription = category.CategoryDescription;
        existing.ParentCategoryId = category.ParentCategoryId;
        existing.IsActive = category.IsActive;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        var existing = await context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
        if (existing is null)
        {
            return false;
        }

        context.Categories.Remove(existing);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> HasNewsArticlesAsync(int categoryId)
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        return await context.NewsArticles.AnyAsync(n => n.CategoryId == categoryId);
    }
}
