using System.ComponentModel.DataAnnotations;

namespace FUNewsManagement.Web.Models.Dtos;

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

    public bool CanEdit { get; set; }
}

public class NewsArticleUpsertDto
{
    [StringLength(400)]
    public string? NewsTitle { get; set; }

    [Required, StringLength(150)]
    public string Headline { get; set; } = string.Empty;

    public string? NewsContent { get; set; }

    public string? NewsSource { get; set; }

    public short? CategoryId { get; set; }

    public bool NewsStatus { get; set; } = true;

    public List<int> TagIds { get; set; } = new();

    public List<string> NewTagNames { get; set; } = new();
}
