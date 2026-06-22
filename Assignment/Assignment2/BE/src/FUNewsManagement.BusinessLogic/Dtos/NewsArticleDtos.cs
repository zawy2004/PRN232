namespace FUNewsManagement.BusinessLogic.Dtos;

public class NewsArticleDto
{
    public string NewsArticleId { get; set; } = string.Empty;

    public string? NewsTitle { get; set; }

    public string Headline { get; set; } = string.Empty;

    public DateTime? CreatedDate { get; set; }

    public string? NewsContent { get; set; }

    public string? NewsSource { get; set; }

    public short? CategoryId { get; set; }

    public string? CategoryName { get; set; }

    /// <summary>
    /// Approval flag (reuses the existing NewsStatus column to avoid a schema change):
    /// false = pending (hidden from guests, still editable), true = approved (visible to guests, locked).
    /// </summary>
    public bool NewsStatus { get; set; }

    public short? CreatedById { get; set; }

    public string? CreatedByName { get; set; }

    public short? UpdatedById { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public List<TagDto> Tags { get; set; } = new();

    /// <summary>True while the article is still pending (not yet approved); approved articles can no longer be edited.</summary>
    public bool CanEdit { get; set; }
}

public class NewsArticleUpsertDto
{
    public string? NewsTitle { get; set; }

    public string Headline { get; set; } = string.Empty;

    public string? NewsContent { get; set; }

    public string? NewsSource { get; set; }

    public short? CategoryId { get; set; }

    // Note: NewsStatus (approval) is intentionally NOT settable here - new articles always start
    // pending and only the Admin Approve action can publish them. Staff cannot self-publish.

    public List<int> TagIds { get; set; } = new();

    /// <summary>New tag names to create (e.g. quick-add) and attach alongside TagIds.</summary>
    public List<string> NewTagNames { get; set; } = new();
}
