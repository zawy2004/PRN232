using FUNewsManagement.DataAccess.Entities;

namespace FUNewsManagement.DataAccess.Repositories;

public interface ICategoryRepository : IGenericRepository<Category, short>
{
    Task<List<Category>> GetAllWithChildrenAsync();

    Task<bool> HasNewsArticlesAsync(short categoryId);

    Task<bool> HasChildCategoriesAsync(short categoryId);
}
