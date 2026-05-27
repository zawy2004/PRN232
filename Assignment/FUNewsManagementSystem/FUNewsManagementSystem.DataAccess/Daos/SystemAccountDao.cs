using FUNewsManagementSystem.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.DataAccess.Daos;

public sealed class SystemAccountDao
{
    private static readonly Lazy<SystemAccountDao> LazyInstance = new(() => new SystemAccountDao());

    public static SystemAccountDao Instance => LazyInstance.Value;

    private SystemAccountDao()
    {
    }

    public async Task<List<SystemAccount>> GetAllAsync()
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        return await context.SystemAccounts.AsNoTracking().ToListAsync();
    }

    public async Task<SystemAccount?> GetByIdAsync(int id)
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        return await context.SystemAccounts.AsNoTracking().FirstOrDefaultAsync(a => a.AccountId == id);
    }

    public async Task<SystemAccount?> GetByEmailAndPasswordAsync(string email, string password)
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        return await context.SystemAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AccountEmail == email && a.AccountPassword == password);
    }

    public async Task<SystemAccount> AddAsync(SystemAccount account)
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        context.SystemAccounts.Add(account);
        await context.SaveChangesAsync();
        return account;
    }

    public async Task<bool> UpdateAsync(SystemAccount account)
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        var existing = await context.SystemAccounts.FirstOrDefaultAsync(a => a.AccountId == account.AccountId);
        if (existing is null)
        {
            return false;
        }

        existing.AccountName = account.AccountName;
        existing.AccountEmail = account.AccountEmail;
        existing.AccountPassword = account.AccountPassword;
        existing.AccountRole = account.AccountRole;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        var existing = await context.SystemAccounts.FirstOrDefaultAsync(a => a.AccountId == id);
        if (existing is null)
        {
            return false;
        }

        context.SystemAccounts.Remove(existing);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> HasNewsArticlesAsync(int accountId)
    {
        using var context = DbContextFactory.Instance.CreateDbContext();
        return await context.NewsArticles.AnyAsync(n => n.CreatedById == accountId);
    }
}
