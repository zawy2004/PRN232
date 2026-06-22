namespace FUNewsManagement.BusinessLogic.Dtos;

public class TagDto
{
    public int TagId { get; set; }

    public string TagName { get; set; } = string.Empty;

    public string? Note { get; set; }
}

public class TagUpsertDto
{
    public string TagName { get; set; } = string.Empty;

    public string? Note { get; set; }
}
