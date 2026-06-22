using System.ComponentModel.DataAnnotations;

namespace FUNewsManagement.Web.Models.Dtos;

public class AccountDto
{
    public short AccountId { get; set; }

    public string AccountName { get; set; } = string.Empty;

    public string AccountEmail { get; set; } = string.Empty;

    public int AccountRole { get; set; }

    public string RoleName { get; set; } = string.Empty;
}

public class AccountUpsertDto
{
    [Required, StringLength(100)]
    public string AccountName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(70)]
    public string AccountEmail { get; set; } = string.Empty;

    [Required]
    public int AccountRole { get; set; }

    public string? Password { get; set; }
}
