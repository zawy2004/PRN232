using FUNewsManagementSystem.DataAccess.Entities;

namespace FUNewsManagementSystem.Repositories.Interfaces;

public interface INewsArticleRepository
{
    Task<IReadOnlyList<NewsArticle>> GetAllAsync(bool includeTags);
    Task<NewsArticle?> GetByIdAsync(int id, bool includeTags);
    Task<NewsArticle> AddAsync(NewsArticle article, IReadOnlyCollection<int> tagIds);
    Task<bool> UpdateAsync(int id, NewsArticle article, IReadOnlyCollection<int> tagIds);
    Task<bool> DeleteAsync(int id);
    Task<IReadOnlyList<NewsArticle>> GetByCreatorAsync(int accountId);
}
