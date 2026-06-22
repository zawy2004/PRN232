using FUNewsManagement.Web.Models.Dtos;

namespace FUNewsManagement.Web.Models.ViewModels;

public class NewsManageViewModel
{
    public List<NewsArticleDto> News { get; set; } = new();

    public List<CategoryDto> Categories { get; set; } = new();

    public List<TagDto> AllTags { get; set; } = new();

    public string? Keyword { get; set; }

    public int? TagId { get; set; }

    public bool MineOnly { get; set; }
}
