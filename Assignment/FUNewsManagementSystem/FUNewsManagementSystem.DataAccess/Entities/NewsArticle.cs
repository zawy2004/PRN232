using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.DataAccess.Entities;

public class NewsArticle
{
    [Key]
    public int NewsArticleId { get; set; }

    [Required]
    [MaxLength(250)]
    public string NewsTitle { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string Headline { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(4000)]
    public string NewsContent { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? NewsSource { get; set; }

    public int CategoryId { get; set; }

    public bool NewsStatus { get; set; } = true;

    public int CreatedById { get; set; }

    public int? UpdatedById { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public Category? Category { get; set; }

    public SystemAccount? CreatedBy { get; set; }

    public SystemAccount? UpdatedBy { get; set; }

    public ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
}
