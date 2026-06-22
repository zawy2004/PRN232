using System.Net.Http.Json;
using FUNewsManagement.Web.Common;
using FUNewsManagement.Web.Models.Dtos;

namespace FUNewsManagement.Web.Services.ApiClients;

public class TagApiClient : ITagApiClient
{
    private readonly HttpClient _httpClient;

    public TagApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TagDto>> GetAllAsync() =>
        await _httpClient.GetJsonSafeAsync<List<TagDto>>("api/tags") ?? new();

    public async Task<TagDto?> GetByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/tags/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        await response.EnsureApiSuccessAsync();
        return await response.Content.ReadFromJsonAsync<TagDto>(JsonOptions.Default);
    }

    public async Task<TagDto?> CreateAsync(TagUpsertDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/tags", dto);
        await response.EnsureApiSuccessAsync();
        return await response.Content.ReadFromJsonAsync<TagDto>(JsonOptions.Default);
    }

    public async Task UpdateAsync(int id, TagUpsertDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/tags/{id}", dto);
        await response.EnsureApiSuccessAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/tags/{id}");
        await response.EnsureApiSuccessAsync();
    }
}
