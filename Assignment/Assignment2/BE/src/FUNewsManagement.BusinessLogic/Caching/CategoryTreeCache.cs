using FUNewsManagement.DataAccess.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace FUNewsManagement.BusinessLogic.Caching;

public class CategoryTreeCache : ICategoryTreeCache
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private List<CategoryNode>? _tree;

    public CategoryTreeCache(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<List<CategoryNode>> GetTreeAsync()
    {
        if (_tree is not null) return _tree;

        await _lock.WaitAsync();
        try
        {
            if (_tree is not null) return _tree;

            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var categories = await unitOfWork.Categories.GetAllWithChildrenAsync();

            var nodesById = categories.ToDictionary(c => c.CategoryId, c => new CategoryNode
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                CategoryDescription = c.CategoryDesciption,
                ParentCategoryId = c.ParentCategoryId,
                IsActive = c.IsActive ?? false,
            });

            var roots = new List<CategoryNode>();
            foreach (var node in nodesById.Values)
            {
                if (node.ParentCategoryId.HasValue && nodesById.TryGetValue(node.ParentCategoryId.Value, out var parent))
                {
                    parent.Children.Add(node);
                }
                else
                {
                    roots.Add(node);
                }
            }

            _tree = roots;
            return _tree;
        }
        finally
        {
            _lock.Release();
        }
    }

    public void Invalidate() => _tree = null;
}
