using FUNewsManagementSystem.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.DataAccess.Daos;

public sealed class NewsArticleDao
{
    private static readonly Lazy<NewsArticleDao> LazyInstance = new(() => new NewsArticleDao());

    public static NewsArticleDao Instance => LazyInstance.Value;

    private NewsArticleDao()
    {
    }

    public async Task<List<NewsArticle>> GetAllAsync(bool includeTags)
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        IQueryable<NewsArticle> query = context.NewsArticles;

        if (includeTags)
        {
            query = query
                .Include(n => n.NewsTags)
                .ThenInclude(nt => nt.Tag)
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.UpdatedBy);
        }

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<NewsArticle?> GetByIdAsync(int id, bool includeTags)
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        IQueryable<NewsArticle> query = context.NewsArticles;

        if (includeTags)
        {
            query = query
                .Include(n => n.NewsTags)
                .ThenInclude(nt => nt.Tag)
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.UpdatedBy);
        }

        return await query.AsNoTracking().FirstOrDefaultAsync(n => n.NewsArticleId == id);
    }

    public async Task<NewsArticle> AddWithTagsAsync(NewsArticle article, IReadOnlyCollection<int> tagIds)
    {
        using var context = DbContextFactory.Instance.CreateDbContext();

        if (tagIds.Count > 0)
        {
            var existingTags = await context.Tags
                .Where(t => tagIds.Contains(t.TagId))
                .Select(t => t.TagId)
                .ToListAsync();

            if (existingTags.Count != tagIds.Count)
            {
                throw new InvalidOperationException("One or more tag IDs do not exist.");
            }

            foreach (var tagId in tagIds)
            {
                article.NewsTags.Add(new NewsTag { TagId = tagId });
            }
        }

        context.NewsArticles.Add(article);
        await context.SaveChangesAsync();
        return article;
    }

    public async Task<bool> UpdateWithTagsAsync(int id, NewsArticle article, IReadOnlyCollection<int> tagIds)
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        var existing = await context.NewsArticles
            .Include(n => n.NewsTags)
            .FirstOrDefaultAsync(n => n.NewsArticleId == id);

        if (existing is null)
        {
            return false;
        }

        existing.NewsTitle = article.NewsTitle;
        existing.Headline = article.Headline;
        existing.NewsContent = article.NewsContent;
        existing.NewsSource = article.NewsSource;
        existing.CategoryId = article.CategoryId;
        existing.NewsStatus = article.NewsStatus;
        existing.UpdatedById = article.UpdatedById;
        existing.ModifiedDate = article.ModifiedDate;

        context.NewsTags.RemoveRange(existing.NewsTags);
        existing.NewsTags.Clear();

        if (tagIds.Count > 0)
        {
            var existingTags = await context.Tags
                .Where(t => tagIds.Contains(t.TagId))
                .Select(t => t.TagId)
                .ToListAsync();

            if (existingTags.Count != tagIds.Count)
            {
                throw new InvalidOperationException("One or more tag IDs do not exist.");
            }

            foreach (var tagId in tagIds)
            {
                existing.NewsTags.Add(new NewsTag { TagId = tagId, NewsArticleId = existing.NewsArticleId });
            }
        }

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        var existing = await context.NewsArticles.FirstOrDefaultAsync(n => n.NewsArticleId == id);
        if (existing is null)
        {
            return false;
        }

        context.NewsArticles.Remove(existing);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<NewsArticle>> GetByCreatorAsync(int accountId)
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        return await context.NewsArticles
            .AsNoTracking()
            .Where(n => n.CreatedById == accountId)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }
}
