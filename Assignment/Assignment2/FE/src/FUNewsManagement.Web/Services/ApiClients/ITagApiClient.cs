using FUNewsManagement.Web.Models.Dtos;

namespace FUNewsManagement.Web.Services.ApiClients;

public interface ITagApiClient
{
    Task<List<TagDto>> GetAllAsync();

    Task<TagDto?> GetByIdAsync(int id);

    Task<TagDto?> CreateAsync(TagUpsertDto dto);

    Task UpdateAsync(int id, TagUpsertDto dto);

    Task DeleteAsync(int id);
}
