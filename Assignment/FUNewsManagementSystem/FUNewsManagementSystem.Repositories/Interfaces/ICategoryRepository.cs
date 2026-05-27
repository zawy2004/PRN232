using FUNewsManagementSystem.DataAccess.Entities;

namespace FUNewsManagementSystem.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category> AddAsync(Category category);
    Task<bool> UpdateAsync(Category category);
    Task<bool> DeleteAsync(int id);
    Task<bool> CanDeleteAsync(int id);
}
