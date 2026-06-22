using FUCourseManagementSystem.BusinessObjects;
using FUCourseManagementSystem.DataAccess;
using FUCourseManagementSystem.DataAccess.DAOs;
using FUCourseManagementSystem.Repositories.Interfaces;

namespace FUCourseManagementSystem.Repositories.Implementations;

public class SystemAccountRepository : ISystemAccountRepository
{
    private readonly SystemAccountDAO _dao;

    public SystemAccountRepository(AppDbContext context)
        => _dao = new SystemAccountDAO(context);

    public IQueryable<SystemAccount> GetAll() => _dao.GetAll();
    public Task<SystemAccount?> GetByIdAsync(int id) => _dao.GetByIdAsync(id);
    public Task<SystemAccount?> GetByEmailAsync(string email) => _dao.GetByEmailAsync(email);
    public Task AddAsync(SystemAccount account) => _dao.AddAsync(account);
    public Task UpdateAsync(SystemAccount account) => _dao.UpdateAsync(account);
    public Task DeleteAsync(SystemAccount account) => _dao.DeleteAsync(account);
}
