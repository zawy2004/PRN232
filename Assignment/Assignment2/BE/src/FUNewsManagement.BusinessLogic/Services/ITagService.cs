using FUNewsManagement.BusinessLogic.Dtos;

namespace FUNewsManagement.BusinessLogic.Services;

public interface ITagService
{
    Task<List<TagDto>> GetAllAsync();

    Task<TagDto?> GetByIdAsync(int id);

    Task<TagDto> CreateAsync(TagUpsertDto dto);

    Task UpdateAsync(int id, TagUpsertDto dto);

    Task DeleteAsync(int id);

    /// <summary>Finds an existing tag by name (case-insensitive) or creates it. Used for quick-add while editing news.</summary>
    Task<TagDto> GetOrCreateByNameAsync(string tagName);
}
