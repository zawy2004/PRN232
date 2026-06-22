using FUNewsManagement.DataAccess.Entities;

namespace FUNewsManagement.DataAccess.Repositories;

public interface ISystemAccountRepository : IGenericRepository<SystemAccount, short>
{
    Task<SystemAccount?> GetByEmailAsync(string email);

    Task<bool> HasCreatedNewsArticlesAsync(short accountId);

    Task<short> GetNextIdAsync();
}
