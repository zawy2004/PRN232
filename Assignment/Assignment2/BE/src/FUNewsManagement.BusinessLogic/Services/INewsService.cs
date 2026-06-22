using FUNewsManagement.BusinessLogic.Dtos;

namespace FUNewsManagement.BusinessLogic.Services;

public interface INewsService
{
    /// <summary>Guest-facing list: only active news, optional keyword/tag search.</summary>
    Task<List<NewsArticleDto>> GetActiveAsync(string? keyword, int? tagId);

    /// <summary>Guest-facing detail: only returns the article if it is active.</summary>
    Task<NewsArticleDto?> GetActiveByIdAsync(string id);

    /// <summary>Staff/Admin listing, optionally restricted to a single creator (own history) and/or filtered.</summary>
    Task<List<NewsArticleDto>> GetForManagementAsync(string? keyword, int? tagId, short? createdById);

    Task<NewsArticleDto?> GetByIdForManagementAsync(string id);

    Task<NewsArticleDto> CreateAsync(NewsArticleUpsertDto dto, short createdById);

    Task UpdateAsync(string id, NewsArticleUpsertDto dto, short updatedById);

    /// <summary>Admin action: marks a pending article as approved, making it visible to guests and locking it from further edits.</summary>
    Task ApproveAsync(string id);

    Task DeleteAsync(string id);

    Task<List<ReportItemDto>> GetReportAsync(DateTime start, DateTime end);
}
