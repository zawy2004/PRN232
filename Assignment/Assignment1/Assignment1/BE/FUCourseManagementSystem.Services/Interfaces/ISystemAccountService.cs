using FUCourseManagementSystem.BusinessObjects;

namespace FUCourseManagementSystem.Services.Interfaces;

public interface ISystemAccountService
{
    IQueryable<SystemAccount> GetAll();
    Task<SystemAccount?> GetByIdAsync(int id);
    Task<SystemAccount?> AuthenticateAsync(string email, string password);
    Task CreateAsync(SystemAccount account);
    Task UpdateAsync(SystemAccount account);
    Task DeleteAsync(int id);
}
