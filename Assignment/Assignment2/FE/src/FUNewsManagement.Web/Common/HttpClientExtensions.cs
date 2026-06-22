using System.Net.Http.Json;

namespace FUNewsManagement.Web.Common;

public static class HttpClientExtensions
{
    /// <summary>
    /// GET + deserialize that throws our own ApiException (with the BE's error message and status code)
    /// instead of HttpClientJsonExtensions.GetFromJsonAsync's raw HttpRequestException on non-success.
    /// </summary>
    public static async Task<T?> GetJsonSafeAsync<T>(this HttpClient httpClient, string requestUri)
    {
        var response = await httpClient.GetAsync(requestUri);
        await response.EnsureApiSuccessAsync();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions.Default);
    }
}
