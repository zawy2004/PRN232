using FUNewsManagement.Web.Models.Dtos;

namespace FUNewsManagement.Web.Models.ViewModels;

public class NewsFormViewModel
{
    public string? NewsArticleId { get; set; }

    public NewsArticleUpsertDto News { get; set; } = new();

    public List<CategoryDto> Categories { get; set; } = new();

    public List<TagDto> AllTags { get; set; } = new();

    /// <summary>Comma-separated free-text tag names to quick-create alongside the picked TagIds.</summary>
    public string NewTagNamesInput { get; set; } = string.Empty;

    public bool CanEdit { get; set; } = true;
}
