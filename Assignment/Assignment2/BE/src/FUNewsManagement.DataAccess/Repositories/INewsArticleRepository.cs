using FUNewsManagement.DataAccess.Entities;

namespace FUNewsManagement.DataAccess.Repositories;

public interface INewsArticleRepository : IGenericRepository<NewsArticle, string>
{
    Task<NewsArticle?> GetWithDetailsAsync(string id);

    IQueryable<NewsArticle> QueryWithDetails();

    Task<string> GetNextIdAsync();
}
