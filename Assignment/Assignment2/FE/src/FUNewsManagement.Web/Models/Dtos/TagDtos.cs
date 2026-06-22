using System.ComponentModel.DataAnnotations;

namespace FUNewsManagement.Web.Models.Dtos;

public class TagDto
{
    public int TagId { get; set; }

    public string TagName { get; set; } = string.Empty;

    public string? Note { get; set; }
}

public class TagUpsertDto
{
    [Required, StringLength(50)]
    public string TagName { get; set; } = string.Empty;

    [StringLength(400)]
    public string? Note { get; set; }
}
