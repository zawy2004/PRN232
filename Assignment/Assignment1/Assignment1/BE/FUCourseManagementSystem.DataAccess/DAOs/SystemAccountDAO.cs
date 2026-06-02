using FUCourseManagementSystem.BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace FUCourseManagementSystem.DataAccess.DAOs;

public class SystemAccountDAO
{
    private static SystemAccountDAO? _instance;
    private static readonly object _lock = new();
    private readonly AppDbContext _context;

    public SystemAccountDAO(AppDbContext context) => _context = context;

    public static SystemAccountDAO Instance(AppDbContext context)
    {
        lock (_lock)
        {
            _instance ??= new SystemAccountDAO(context);
            return _instance;
        }
    }

    public IQueryable<SystemAccount> GetAll() => _context.SystemAccounts.AsNoTracking();

    public async Task<SystemAccount?> GetByIdAsync(int id)
        => await _context.SystemAccounts.FindAsync(id);

    public async Task<SystemAccount?> GetByEmailAsync(string email)
        => await _context.SystemAccounts.AsNoTracking()
            .FirstOrDefaultAsync(a => a.AccountEmail == email);

    public async Task AddAsync(SystemAccount account)
    {
        await _context.SystemAccounts.AddAsync(account);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(SystemAccount account)
    {
        _context.SystemAccounts.Update(account);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(SystemAccount account)
    {
        _context.SystemAccounts.Remove(account);
        await _context.SaveChangesAsync();
    }
}
