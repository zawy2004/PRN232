using FUCourseManagementSystem.BusinessObjects;
using FUCourseManagementSystem.Repositories.Interfaces;
using FUCourseManagementSystem.Services.Interfaces;

namespace FUCourseManagementSystem.Services.Implementations;

public class SystemAccountService : ISystemAccountService
{
    private readonly ISystemAccountRepository _repo;

    public SystemAccountService(ISystemAccountRepository repo) => _repo = repo;

    public IQueryable<SystemAccount> GetAll() => _repo.GetAll();

    public Task<SystemAccount?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

    public async Task<SystemAccount?> AuthenticateAsync(string email, string password)
    {
        var account = await _repo.GetByEmailAsync(email);
        if (account == null || !account.IsActive) return null;
        return account.AccountPassword == password ? account : null;
    }

    public async Task CreateAsync(SystemAccount account)
    {
        var existing = await _repo.GetByEmailAsync(account.AccountEmail);
        if (existing != null)
            throw new InvalidOperationException("Email already in use.");
        await _repo.AddAsync(account);
    }

    public async Task UpdateAsync(SystemAccount account)
    {
        var existing = await _repo.GetByIdAsync(account.AccountID)
            ?? throw new KeyNotFoundException("Account not found.");
        existing.AccountName = account.AccountName;
        existing.AccountEmail = account.AccountEmail;
        existing.AccountRole = account.AccountRole;
        existing.IsActive = account.IsActive;
        if (!string.IsNullOrWhiteSpace(account.AccountPassword))
            existing.AccountPassword = account.AccountPassword;
        await _repo.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        var account = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Account not found.");
        await _repo.DeleteAsync(account);
    }
}
