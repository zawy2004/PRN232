using FUCourseManagementSystem.BusinessObjects;

namespace FUCourseManagementSystem.Repositories.Interfaces;

public interface ISystemAccountRepository
{
    IQueryable<SystemAccount> GetAll();
    Task<SystemAccount?> GetByIdAsync(int id);
    Task<SystemAccount?> GetByEmailAsync(string email);
    Task AddAsync(SystemAccount account);
    Task UpdateAsync(SystemAccount account);
    Task DeleteAsync(SystemAccount account);
}
