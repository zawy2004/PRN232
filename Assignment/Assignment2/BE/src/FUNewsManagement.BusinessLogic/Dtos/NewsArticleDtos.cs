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

    public bool NewsStatus { get; set; }

    public short? CreatedById { get; set; }

    public string? CreatedByName { get; set; }

    public short? UpdatedById { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public List<TagDto> Tags { get; set; } = new();

    /// <summary>True if the 3-minute edit window has not elapsed (always true for Admin).</summary>
    public bool CanEdit { get; set; }
}

public class NewsArticleUpsertDto
{
    public string? NewsTitle { get; set; }

    public string Headline { get; set; } = string.Empty;

    public string? NewsContent { get; set; }

    public string? NewsSource { get; set; }

    public short? CategoryId { get; set; }

    public bool NewsStatus { get; set; } = true;

    public List<int> TagIds { get; set; } = new();

    /// <summary>New tag names to create (e.g. quick-add) and attach alongside TagIds.</summary>
    public List<string> NewTagNames { get; set; } = new();
}
