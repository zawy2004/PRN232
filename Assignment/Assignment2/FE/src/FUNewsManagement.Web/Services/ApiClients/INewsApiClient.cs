using FUNewsManagement.Web.Models.Dtos;

namespace FUNewsManagement.Web.Services.ApiClients;

public interface INewsApiClient
{
    /// <summary>Guest-facing search over active news via OData (keyword across title/headline/tags, a specific tag, and/or category).</summary>
    Task<List<NewsArticleDto>> GetActiveAsync(string? keyword, int? tagId, short? categoryId = null);

    Task<NewsArticleDto?> GetActiveByIdAsync(string id);

    Task<List<NewsArticleDto>> GetForManagementAsync(string? keyword, int? tagId, bool mineOnly);

    Task<NewsArticleDto?> GetByIdForManagementAsync(string id);

    Task<NewsArticleDto?> CreateAsync(NewsArticleUpsertDto dto);

    Task UpdateAsync(string id, NewsArticleUpsertDto dto);

    /// <summary>Admin-only: approve a pending article so guests can see it.</summary>
    Task ApproveAsync(string id);

    Task DeleteAsync(string id);
}
