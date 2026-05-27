using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.DataAccess.Entities;

public class Tag
{
    [Key]
    public int TagId { get; set; }

    [Required]
    [MaxLength(100)]
    public string TagName { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Note { get; set; }

    public ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
}
