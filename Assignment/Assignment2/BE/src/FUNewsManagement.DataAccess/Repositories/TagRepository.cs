using FUNewsManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagement.DataAccess.Repositories;

public class TagRepository : GenericRepository<Tag, int>, ITagRepository
{
    public TagRepository(FUNewsManagementContext context) : base(context)
    {
    }

    public async Task<Tag?> GetByNameAsync(string tagName) =>
        await DbSet.FirstOrDefaultAsync(t => t.TagName == tagName);

    public async Task<bool> IsUsedByNewsArticleAsync(int tagId) =>
        await DbSet.Where(t => t.TagId == tagId).SelectMany(t => t.NewsArticles).AnyAsync();

    public async Task<int> GetNextIdAsync()
    {
        var ids = await DbSet.Select(t => t.TagId).ToListAsync();
        return (ids.Count == 0 ? 0 : ids.Max()) + 1;
    }
}
