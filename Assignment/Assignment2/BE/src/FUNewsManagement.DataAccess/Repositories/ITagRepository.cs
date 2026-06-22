using FUNewsManagement.DataAccess.Entities;

namespace FUNewsManagement.DataAccess.Repositories;

public interface ITagRepository : IGenericRepository<Tag, int>
{
    Task<Tag?> GetByNameAsync(string tagName);

    Task<bool> IsUsedByNewsArticleAsync(int tagId);

    Task<int> GetNextIdAsync();
}
