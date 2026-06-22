using FUNewsManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagement.DataAccess.Repositories;

public class SystemAccountRepository : GenericRepository<SystemAccount, short>, ISystemAccountRepository
{
    public SystemAccountRepository(FUNewsManagementContext context) : base(context)
    {
    }

    public async Task<SystemAccount?> GetByEmailAsync(string email) =>
        await DbSet.FirstOrDefaultAsync(a => a.AccountEmail != null && a.AccountEmail.ToLower() == email.ToLower());

    public async Task<bool> HasCreatedNewsArticlesAsync(short accountId) =>
        await Context.NewsArticles.AnyAsync(n => n.CreatedById == accountId);

    public async Task<short> GetNextIdAsync()
    {
        var ids = await DbSet.Select(a => a.AccountId).ToListAsync();
        return (short)((ids.Count == 0 ? 0 : ids.Max()) + 1);
    }
}
