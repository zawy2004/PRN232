namespace FUNewsManagement.BusinessLogic.Options;

public class AdminAccountOptions
{
    public short AccountId { get; set; } = 0;

    public string AccountName { get; set; } = "Admin";

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
