using FUNewsManagement.DataAccess.Repositories;

namespace FUNewsManagement.DataAccess.UnitOfWork;

public interface IUnitOfWork
{
    ICategoryRepository Categories { get; }

    INewsArticleRepository NewsArticles { get; }

    ITagRepository Tags { get; }

    ISystemAccountRepository Accounts { get; }

    Task<int> SaveChangesAsync();
}
