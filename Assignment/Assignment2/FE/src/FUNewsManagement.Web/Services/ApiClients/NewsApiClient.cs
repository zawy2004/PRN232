using System.Net.Http.Json;
using System.Text;
using FUNewsManagement.Web.Common;
using FUNewsManagement.Web.Models.Dtos;

namespace FUNewsManagement.Web.Services.ApiClients;

public class NewsApiClient : INewsApiClient
{
    private readonly HttpClient _httpClient;

    public NewsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<NewsArticleDto>> GetActiveAsync(string? keyword, int? tagId, short? categoryId = null)
    {
        var filters = new List<string>();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var escaped = EscapeODataString(keyword);
            filters.Add($"(contains(NewsTitle,'{escaped}') or contains(Headline,'{escaped}') or Tags/any(t: contains(t/TagName,'{escaped}')))");
        }

        if (tagId.HasValue)
        {
            filters.Add($"Tags/any(t: t/TagId eq {tagId.Value})");
        }

        if (categoryId.HasValue)
        {
            filters.Add($"CategoryId eq {categoryId.Value}");
        }

        var query = new StringBuilder("odata/NewsArticles?$expand=Tags&$orderby=CreatedDate desc");
        if (filters.Count > 0)
        {
            query.Append("&$filter=").Append(Uri.EscapeDataString(string.Join(" and ", filters)));
        }

        var result = await _httpClient.GetJsonSafeAsync<ODataResponse<NewsArticleDto>>(query.ToString());
        return result?.Value ?? new();
    }

    public async Task<NewsArticleDto?> GetActiveByIdAsync(string id)
    {
        var encodedId = Uri.EscapeDataString(id);
        var response = await _httpClient.GetAsync($"odata/NewsArticles('{encodedId}')?$expand=Tags");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        await response.EnsureApiSuccessAsync();
        return await response.Content.ReadFromJsonAsync<NewsArticleDto>(JsonOptions.Default);
    }

    public async Task<List<NewsArticleDto>> GetForManagementAsync(string? keyword, int? tagId, bool mineOnly)
    {
        var query = new StringBuilder("api/news/manage?");
        if (!string.IsNullOrWhiteSpace(keyword)) query.Append($"keyword={Uri.EscapeDataString(keyword)}&");
        if (tagId.HasValue) query.Append($"tagId={tagId.Value}&");
        query.Append($"mineOnly={mineOnly}");

        return await _httpClient.GetJsonSafeAsync<List<NewsArticleDto>>(query.ToString()) ?? new();
    }

    public async Task<NewsArticleDto?> GetByIdForManagementAsync(string id)
    {
        var response = await _httpClient.GetAsync($"api/news/manage/{Uri.EscapeDataString(id)}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        await response.EnsureApiSuccessAsync();
        return await response.Content.ReadFromJsonAsync<NewsArticleDto>(JsonOptions.Default);
    }

    public async Task<NewsArticleDto?> CreateAsync(NewsArticleUpsertDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/news", dto);
        await response.EnsureApiSuccessAsync();
        return await response.Content.ReadFromJsonAsync<NewsArticleDto>(JsonOptions.Default);
    }

    public async Task UpdateAsync(string id, NewsArticleUpsertDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/news/{Uri.EscapeDataString(id)}", dto);
        await response.EnsureApiSuccessAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"api/news/{Uri.EscapeDataString(id)}");
        await response.EnsureApiSuccessAsync();
    }

    private static string EscapeODataString(string value) => value.Replace("'", "''");
}
