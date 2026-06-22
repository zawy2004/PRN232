namespace FUNewsManagement.BusinessLogic.Caching;

public class CategoryNode
{
    public short CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string CategoryDescription { get; set; } = string.Empty;

    public short? ParentCategoryId { get; set; }

    public bool IsActive { get; set; }

    public List<CategoryNode> Children { get; set; } = new();
}
