using FUNewsManagement.BusinessLogic.Dtos;

namespace FUNewsManagement.BusinessLogic.Services;

public interface ICategoryService
{
    Task<List<CategoryTreeDto>> GetTreeAsync();

    Task<List<CategoryDto>> GetAllAsync();

    Task<CategoryDto?> GetByIdAsync(short id);

    Task<CategoryDto> CreateAsync(CategoryUpsertDto dto);

    Task UpdateAsync(short id, CategoryUpsertDto dto);

    Task DeleteAsync(short id);
}
