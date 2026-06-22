using FUNewsManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagement.DataAccess.Repositories;

public class NewsArticleRepository : GenericRepository<NewsArticle, string>, INewsArticleRepository
{
    public NewsArticleRepository(FUNewsManagementContext context) : base(context)
    {
    }

    public IQueryable<NewsArticle> QueryWithDetails() =>
        DbSet.Include(n => n.Category)
             .Include(n => n.CreatedBy)
             .Include(n => n.Tags)
             .AsQueryable();

    public async Task<NewsArticle?> GetWithDetailsAsync(string id) =>
        await QueryWithDetails().FirstOrDefaultAsync(n => n.NewsArticleId == id);

    public async Task<string> GetNextIdAsync()
    {
        var ids = await DbSet.Select(n => n.NewsArticleId).ToListAsync();
        var maxNumeric = ids
            .Select(i => int.TryParse(i, out var v) ? v : 0)
            .DefaultIfEmpty(0)
            .Max();
        return (maxNumeric + 1).ToString();
    }
}
