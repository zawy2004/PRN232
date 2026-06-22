using System.Net.Http.Json;
using FUNewsManagement.Web.Common;
using FUNewsManagement.Web.Models.Dtos;

namespace FUNewsManagement.Web.Services.ApiClients;

public class AccountApiClient : IAccountApiClient
{
    private readonly HttpClient _httpClient;

    public AccountApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<AccountDto>> GetAllAsync() =>
        await _httpClient.GetJsonSafeAsync<List<AccountDto>>("api/accounts") ?? new();

    public async Task<AccountDto?> GetByIdAsync(short id)
    {
        var response = await _httpClient.GetAsync($"api/accounts/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        await response.EnsureApiSuccessAsync();
        return await response.Content.ReadFromJsonAsync<AccountDto>(JsonOptions.Default);
    }

    public async Task<AccountDto?> CreateAsync(AccountUpsertDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/accounts", dto);
        await response.EnsureApiSuccessAsync();
        return await response.Content.ReadFromJsonAsync<AccountDto>(JsonOptions.Default);
    }

    public async Task UpdateAsync(short id, AccountUpsertDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/accounts/{id}", dto);
        await response.EnsureApiSuccessAsync();
    }

    public async Task DeleteAsync(short id)
    {
        var response = await _httpClient.DeleteAsync($"api/accounts/{id}");
        await response.EnsureApiSuccessAsync();
    }
}
