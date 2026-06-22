using System.ComponentModel.DataAnnotations;

namespace FUNewsManagement.Web.Models.Dtos;

public class LoginRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class GoogleLoginRequestDto
{
    public string IdToken { get; set; } = string.Empty;
}

public class LoginResultDto
{
    public short AccountId { get; set; }

    public string AccountName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}
