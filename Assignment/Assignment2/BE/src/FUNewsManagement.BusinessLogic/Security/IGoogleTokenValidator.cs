namespace FUNewsManagement.BusinessLogic.Security;

public record GoogleIdentity(string Email, string? Name);

public interface IGoogleTokenValidator
{
    /// <summary>Validates a Google ID token and returns the verified email + display name, or null if invalid.</summary>
    Task<GoogleIdentity?> ValidateAsync(string idToken);
}
