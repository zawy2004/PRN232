namespace FUNewsManagement.BusinessLogic.Dtos;

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
    public string AccountName { get; set; } = string.Empty;

    public string AccountEmail { get; set; } = string.Empty;

    public int AccountRole { get; set; }

    /// <summary>Plaintext password from the client; only required on create or when changing it.</summary>
    public string? Password { get; set; }
}
