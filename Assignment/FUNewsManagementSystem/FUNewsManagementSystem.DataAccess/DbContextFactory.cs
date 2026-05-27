using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.DataAccess;

public sealed class DbContextFactory
{
    private static readonly Lazy<DbContextFactory> LazyInstance = new(() => new DbContextFactory());

    public static DbContextFactory Instance => LazyInstance.Value;

    private DbContextOptions<FunewsDbContext>? _options;

    private DbContextFactory()
    {
    }

    public void SetOptions(DbContextOptions<FunewsDbContext> options)
    {
        _options = options;
    }

    public FunewsDbContext CreateDbContext()
    {
        if (_options is null)
        {
            throw new InvalidOperationException("DbContext options have not been configured.");
        }

        return new FunewsDbContext(_options);
    }
}
