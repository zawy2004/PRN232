using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.DataAccess.Entities;

public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required]
    [MaxLength(150)]
    public string CategoryName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? CategoryDescription { get; set; }

    public int? ParentCategoryId { get; set; }

    public bool IsActive { get; set; } = true;

    public Category? ParentCategory { get; set; }

    public ICollection<Category> SubCategories { get; set; } = new List<Category>();

    public ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}
