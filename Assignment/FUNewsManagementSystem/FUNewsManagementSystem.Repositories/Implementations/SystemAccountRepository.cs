using FUNewsManagementSystem.DataAccess.Daos;
using FUNewsManagementSystem.DataAccess.Entities;
using FUNewsManagementSystem.Repositories.Interfaces;

namespace FUNewsManagementSystem.Repositories.Implementations;

public class SystemAccountRepository : ISystemAccountRepository
{
    public async Task<IReadOnlyList<SystemAccount>> GetAllAsync()
    {
        return await SystemAccountDao.Instance.GetAllAsync();
    }

    public async Task<SystemAccount?> GetByIdAsync(int id)
    {
        return await SystemAccountDao.Instance.GetByIdAsync(id);
    }

    public async Task<SystemAccount?> GetByEmailAndPasswordAsync(string email, string password)
    {
        return await SystemAccountDao.Instance.GetByEmailAndPasswordAsync(email, password);
    }

    public async Task<SystemAccount> AddAsync(SystemAccount account)
    {
        return await SystemAccountDao.Instance.AddAsync(account);
    }

    public async Task<bool> UpdateAsync(SystemAccount account)
    {
        return await SystemAccountDao.Instance.UpdateAsync(account);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (!await CanDeleteAsync(id))
        {
            return false;
        }

        return await SystemAccountDao.Instance.DeleteAsync(id);
    }

    public async Task<bool> CanDeleteAsync(int id)
    {
        return !await SystemAccountDao.Instance.HasNewsArticlesAsync(id);
    }
}
