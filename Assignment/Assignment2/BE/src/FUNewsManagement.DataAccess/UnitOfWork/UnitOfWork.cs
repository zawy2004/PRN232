using FUNewsManagement.DataAccess.Repositories;

namespace FUNewsManagement.DataAccess.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly FUNewsManagementContext _context;

    public UnitOfWork(
        FUNewsManagementContext context,
        ICategoryRepository categories,
        INewsArticleRepository newsArticles,
        ITagRepository tags,
        ISystemAccountRepository accounts)
    {
        _context = context;
        Categories = categories;
        NewsArticles = newsArticles;
        Tags = tags;
        Accounts = accounts;
    }

    public ICategoryRepository Categories { get; }

    public INewsArticleRepository NewsArticles { get; }

    public ITagRepository Tags { get; }

    public ISystemAccountRepository Accounts { get; }

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
}
