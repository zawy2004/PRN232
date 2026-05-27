using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.DataAccess.Entities;

public class SystemAccount
{
    [Key]
    public int AccountId { get; set; }

    [Required]
    [MaxLength(100)]
    public string AccountName { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    [EmailAddress]
    public string AccountEmail { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string AccountPassword { get; set; } = string.Empty;

    public int AccountRole { get; set; }

    public ICollection<NewsArticle> CreatedNewsArticles { get; set; } = new List<NewsArticle>();

    public ICollection<NewsArticle> UpdatedNewsArticles { get; set; } = new List<NewsArticle>();
}
