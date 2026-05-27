using FUNewsManagementSystem.DataAccess.Daos;
using FUNewsManagementSystem.DataAccess.Entities;
using FUNewsManagementSystem.Repositories.Interfaces;

namespace FUNewsManagementSystem.Repositories.Implementations;

public class NewsArticleRepository : INewsArticleRepository
{
    public async Task<IReadOnlyList<NewsArticle>> GetAllAsync(bool includeTags)
    {
        return await NewsArticleDao.Instance.GetAllAsync(includeTags);
    }

    public async Task<NewsArticle?> GetByIdAsync(int id, bool includeTags)
    {
        return await NewsArticleDao.Instance.GetByIdAsync(id, includeTags);
    }

    public async Task<NewsArticle> AddAsync(NewsArticle article, IReadOnlyCollection<int> tagIds)
    {
        return await NewsArticleDao.Instance.AddWithTagsAsync(article, tagIds);
    }

    public async Task<bool> UpdateAsync(int id, NewsArticle article, IReadOnlyCollection<int> tagIds)
    {
        return await NewsArticleDao.Instance.UpdateWithTagsAsync(id, article, tagIds);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await NewsArticleDao.Instance.DeleteAsync(id);
    }

    public async Task<IReadOnlyList<NewsArticle>> GetByCreatorAsync(int accountId)
    {
        return await NewsArticleDao.Instance.GetByCreatorAsync(accountId);
    }
}
