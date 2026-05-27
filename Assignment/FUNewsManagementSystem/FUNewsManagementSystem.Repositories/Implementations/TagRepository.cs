using FUNewsManagementSystem.DataAccess.Daos;
using FUNewsManagementSystem.DataAccess.Entities;
using FUNewsManagementSystem.Repositories.Interfaces;

namespace FUNewsManagementSystem.Repositories.Implementations;

public class TagRepository : ITagRepository
{
    public async Task<IReadOnlyList<Tag>> GetAllAsync()
    {
        return await TagDao.Instance.GetAllAsync();
    }

    public async Task<Tag?> GetByIdAsync(int id)
    {
        return await TagDao.Instance.GetByIdAsync(id);
    }

    public async Task<Tag> AddAsync(Tag tag)
    {
        return await TagDao.Instance.AddAsync(tag);
    }

    public async Task<bool> UpdateAsync(Tag tag)
    {
        return await TagDao.Instance.UpdateAsync(tag);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await TagDao.Instance.DeleteAsync(id);
    }
}
