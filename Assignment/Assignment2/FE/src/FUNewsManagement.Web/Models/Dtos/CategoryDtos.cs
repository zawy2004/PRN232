using System.ComponentModel.DataAnnotations;

namespace FUNewsManagement.Web.Models.Dtos;

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
    [Required, StringLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    [Required, StringLength(250)]
    public string CategoryDescription { get; set; } = string.Empty;

    public short? ParentCategoryId { get; set; }

    public bool IsActive { get; set; } = true;
}
