using FUNewsManagementSystem.DataAccess.Entities;

namespace FUNewsManagementSystem.Repositories.Interfaces;

public interface ISystemAccountRepository
{
    Task<IReadOnlyList<SystemAccount>> GetAllAsync();
    Task<SystemAccount?> GetByIdAsync(int id);
    Task<SystemAccount?> GetByEmailAndPasswordAsync(string email, string password);
    Task<SystemAccount> AddAsync(SystemAccount account);
    Task<bool> UpdateAsync(SystemAccount account);
    Task<bool> DeleteAsync(int id);
    Task<bool> CanDeleteAsync(int id);
}
