namespace FUNewsManagement.BusinessLogic.Caching;

/// <summary>
/// Singleton in-memory cache of the category hierarchy, used to render the
/// multi-level navigation menu without hitting the database on every request.
/// </summary>
public interface ICategoryTreeCache
{
    Task<List<CategoryNode>> GetTreeAsync();

    void Invalidate();
}
