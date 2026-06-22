using FUCourseManagementSystem.WebClient.Models;
using System.Text;
using System.Text.Json;

namespace FUCourseManagementSystem.WebClient.Services;

public class ApiService
{
    private readonly IHttpClientFactory _factory;

    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new ODataResultConverterFactory() }
    };

    public ApiService(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    private HttpClient CreateClient()
    {
        return _factory.CreateClient("API");
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        var response = await CreateClient().GetAsync(url);
        if (!response.IsSuccessStatusCode) return default;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, _json);
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string url, T body)
    {
        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        return await CreateClient().PostAsync(url, content);
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string url, T body)
    {
        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        return await CreateClient().PutAsync(url, content);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string url)
        => await CreateClient().DeleteAsync(url);
}
