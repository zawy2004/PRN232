namespace FUNewsManagementSystem.DataAccess.Entities;

public class NewsTag
{
    public int NewsArticleId { get; set; }

    public int TagId { get; set; }

    public NewsArticle? NewsArticle { get; set; }

    public Tag? Tag { get; set; }
}
