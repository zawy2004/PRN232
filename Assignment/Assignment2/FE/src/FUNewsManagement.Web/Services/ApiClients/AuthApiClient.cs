using System.Net.Http.Json;
using FUNewsManagement.Web.Common;
using FUNewsManagement.Web.Models.Dtos;

namespace FUNewsManagement.Web.Services.ApiClients;

public class AuthApiClient : IAuthApiClient
{
    private readonly HttpClient _httpClient;

    public AuthApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoginResultDto?> LoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new LoginRequestDto { Email = email, Password = password });
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<LoginResultDto>(JsonOptions.Default);
    }

    public async Task<LoginResultDto?> GoogleLoginAsync(string idToken)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/google", new GoogleLoginRequestDto { IdToken = idToken });
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<LoginResultDto>(JsonOptions.Default);
    }
}
