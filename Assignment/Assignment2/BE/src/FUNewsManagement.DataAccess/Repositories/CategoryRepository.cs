using FUNewsManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagement.DataAccess.Repositories;

public class CategoryRepository : GenericRepository<Category, short>, ICategoryRepository
{
    public CategoryRepository(FUNewsManagementContext context) : base(context)
    {
    }

    public async Task<List<Category>> GetAllWithChildrenAsync() =>
        await DbSet.AsNoTracking().ToListAsync();

    public async Task<bool> HasNewsArticlesAsync(short categoryId) =>
        await Context.NewsArticles.AnyAsync(n => n.CategoryId == categoryId);

    public async Task<bool> HasChildCategoriesAsync(short categoryId) =>
        await DbSet.AnyAsync(c => c.ParentCategoryId == categoryId);
}
