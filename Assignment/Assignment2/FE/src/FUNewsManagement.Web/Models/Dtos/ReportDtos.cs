namespace FUNewsManagement.Web.Models.Dtos;

public class ReportItemDto
{
    public string NewsArticleId { get; set; } = string.Empty;

    public string? NewsTitle { get; set; }

    public string? CategoryName { get; set; }

    public string? CreatedByName { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool NewsStatus { get; set; }
}
