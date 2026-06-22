using FUNewsManagement.Web.Models.Dtos;

namespace FUNewsManagement.Web.Models.ViewModels;

public class NewsListViewModel
{
    public List<NewsArticleDto> News { get; set; } = new();

    public string? Keyword { get; set; }

    public int? TagId { get; set; }

    public short? CategoryId { get; set; }

    public List<TagDto> AllTags { get; set; } = new();
}
