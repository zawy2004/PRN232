using FUNewsManagementSystem.DataAccess.Daos;
using FUNewsManagementSystem.DataAccess.Entities;
using FUNewsManagementSystem.Repositories.Interfaces;

namespace FUNewsManagementSystem.Repositories.Implementations;

public class CategoryRepository : ICategoryRepository
{
    public async Task<IReadOnlyList<Category>> GetAllAsync()
    {
        return await CategoryDao.Instance.GetAllAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await CategoryDao.Instance.GetByIdAsync(id);
    }

    public async Task<Category> AddAsync(Category category)
    {
        return await CategoryDao.Instance.AddAsync(category);
    }

    public async Task<bool> UpdateAsync(Category category)
    {
        return await CategoryDao.Instance.UpdateAsync(category);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (!await CanDeleteAsync(id))
        {
            return false;
        }

        return await CategoryDao.Instance.DeleteAsync(id);
    }

    public async Task<bool> CanDeleteAsync(int id)
    {
        return !await CategoryDao.Instance.HasNewsArticlesAsync(id);
    }
}
