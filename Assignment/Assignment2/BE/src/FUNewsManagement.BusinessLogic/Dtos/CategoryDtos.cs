namespace FUNewsManagement.BusinessLogic.Dtos;

public class CategoryDto
{
    public short CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string CategoryDescription { get; set; } = string.Empty;

    public short? ParentCategoryId { get; set; }

    public bool IsActive { get; set; }
}

public class CategoryTreeDto : CategoryDto
{
    public List<CategoryTreeDto> Children { get; set; } = new();
}

public class CategoryUpsertDto
{
    public string CategoryName { get; set; } = string.Empty;

    public string CategoryDescription { get; set; } = string.Empty;

    public short? ParentCategoryId { get; set; }

    public bool IsActive { get; set; } = true;
}
