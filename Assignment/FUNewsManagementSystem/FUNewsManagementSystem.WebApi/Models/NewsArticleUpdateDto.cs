using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.WebApi.Models;

public class NewsArticleUpdateDto
{
    [Required]
    [MaxLength(250)]
    public string NewsTitle { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string Headline { get; set; } = string.Empty;

    [Required]
    [MaxLength(4000)]
    public string NewsContent { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? NewsSource { get; set; }

    public int CategoryId { get; set; }

    public bool NewsStatus { get; set; }

    public int? UpdatedById { get; set; }

    public IReadOnlyCollection<int> TagIds { get; set; } = Array.Empty<int>();
}
