using System.Net.Http.Json;
using FUNewsManagement.Web.Common;
using FUNewsManagement.Web.Models.Dtos;

namespace FUNewsManagement.Web.Services.ApiClients;

public class CategoryApiClient : ICategoryApiClient
{
    private readonly HttpClient _httpClient;

    public CategoryApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CategoryTreeDto>> GetTreeAsync() =>
        await _httpClient.GetJsonSafeAsync<List<CategoryTreeDto>>("api/categories/tree") ?? new();

    public async Task<List<CategoryDto>> GetAllAsync() =>
        await _httpClient.GetJsonSafeAsync<List<CategoryDto>>("api/categories") ?? new();

    public async Task<CategoryDto?> GetByIdAsync(short id)
    {
        var response = await _httpClient.GetAsync($"api/categories/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        await response.EnsureApiSuccessAsync();
        return await response.Content.ReadFromJsonAsync<CategoryDto>(JsonOptions.Default);
    }

    public async Task<CategoryDto?> CreateAsync(CategoryUpsertDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/categories", dto);
        await response.EnsureApiSuccessAsync();
        return await response.Content.ReadFromJsonAsync<CategoryDto>(JsonOptions.Default);
    }

    public async Task UpdateAsync(short id, CategoryUpsertDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/categories/{id}", dto);
        await response.EnsureApiSuccessAsync();
    }

    public async Task DeleteAsync(short id)
    {
        var response = await _httpClient.DeleteAsync($"api/categories/{id}");
        await response.EnsureApiSuccessAsync();
    }
}
