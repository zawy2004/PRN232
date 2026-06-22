using FUNewsManagement.BusinessLogic.Caching;
using FUNewsManagement.BusinessLogic.Dtos;
using FUNewsManagement.BusinessLogic.Exceptions;
using FUNewsManagement.DataAccess.Entities;
using FUNewsManagement.DataAccess.UnitOfWork;

namespace FUNewsManagement.BusinessLogic.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoryTreeCache _cache;

    public CategoryService(IUnitOfWork unitOfWork, ICategoryTreeCache cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<List<CategoryTreeDto>> GetTreeAsync()
    {
        var nodes = await _cache.GetTreeAsync();
        return nodes.Select(MapNode).ToList();
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllWithChildrenAsync();
        return categories.Select(MapEntity).ToList();
    }

    public async Task<CategoryDto?> GetByIdAsync(short id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        return category is null ? null : MapEntity(category);
    }

    public async Task<CategoryDto> CreateAsync(CategoryUpsertDto dto)
    {
        var entity = new Category
        {
            CategoryName = dto.CategoryName,
            CategoryDesciption = dto.CategoryDescription,
            ParentCategoryId = dto.ParentCategoryId,
            IsActive = dto.IsActive,
        };

        await _unitOfWork.Categories.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        _cache.Invalidate();

        return MapEntity(entity);
    }

    public async Task UpdateAsync(short id, CategoryUpsertDto dto)
    {
        var entity = await _unitOfWork.Categories.GetByIdAsync(id)
            ?? throw new EntityNotFoundException($"Category {id} not found.");

        entity.CategoryName = dto.CategoryName;
        entity.CategoryDesciption = dto.CategoryDescription;
        entity.ParentCategoryId = dto.ParentCategoryId;
        entity.IsActive = dto.IsActive;

        _unitOfWork.Categories.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        _cache.Invalidate();
    }

    public async Task DeleteAsync(short id)
    {
        var entity = await _unitOfWork.Categories.GetByIdAsync(id)
            ?? throw new EntityNotFoundException($"Category {id} not found.");

        if (await _unitOfWork.Categories.HasNewsArticlesAsync(id))
            throw new BusinessRuleException("Cannot delete a category that already has news articles.");

        if (await _unitOfWork.Categories.HasChildCategoriesAsync(id))
            throw new BusinessRuleException("Cannot delete a category that has child categories.");

        _unitOfWork.Categories.Remove(entity);
        await _unitOfWork.SaveChangesAsync();
        _cache.Invalidate();
    }

    private static CategoryDto MapEntity(Category c) => new()
    {
        CategoryId = c.CategoryId,
        CategoryName = c.CategoryName,
        CategoryDescription = c.CategoryDesciption,
        ParentCategoryId = c.ParentCategoryId,
        IsActive = c.IsActive ?? false,
    };

    private static CategoryTreeDto MapNode(CategoryNode n) => new()
    {
        CategoryId = n.CategoryId,
        CategoryName = n.CategoryName,
        CategoryDescription = n.CategoryDescription,
        ParentCategoryId = n.ParentCategoryId,
        IsActive = n.IsActive,
        Children = n.Children.Select(MapNode).ToList(),
    };
}
