using FUNewsManagement.Web.Models.Dtos;

namespace FUNewsManagement.Web.Services.ApiClients;

public interface ICategoryApiClient
{
    Task<List<CategoryTreeDto>> GetTreeAsync();

    Task<List<CategoryDto>> GetAllAsync();

    Task<CategoryDto?> GetByIdAsync(short id);

    Task<CategoryDto?> CreateAsync(CategoryUpsertDto dto);

    Task UpdateAsync(short id, CategoryUpsertDto dto);

    Task DeleteAsync(short id);
}
