using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FUNewsManagement.Api.Security;

/// <summary>
/// Lightweight stand-in for JWT while the project doesn't need token-based auth yet: trusts the
/// caller's identity headers (set by the FE after FE's own cookie login - see
/// FUNewsManagement.Web/Common/IdentityForwardingHandler.cs) instead of validating a signed token.
/// There is no signature/expiry check here, so this only works because the FE is the sole caller
/// in this setup. Swap this handler for JWT (or another verifiable scheme) before exposing the API
/// to untrusted clients.
/// </summary>
public class HeaderIdentityAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "HeaderIdentity";

    public const string AccountIdHeader = "X-Account-Id";
    public const string AccountNameHeader = "X-Account-Name";
    public const string AccountEmailHeader = "X-Account-Email";
    public const string AccountRoleHeader = "X-Account-Role";

    public HeaderIdentityAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var accountId = Request.Headers[AccountIdHeader].ToString();
        if (string.IsNullOrEmpty(accountId))
            return Task.FromResult(AuthenticateResult.NoResult());

        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, accountId) };

        var name = Request.Headers[AccountNameHeader].ToString();
        if (!string.IsNullOrEmpty(name)) claims.Add(new(ClaimTypes.Name, name));

        var email = Request.Headers[AccountEmailHeader].ToString();
        if (!string.IsNullOrEmpty(email)) claims.Add(new(ClaimTypes.Email, email));

        var role = Request.Headers[AccountRoleHeader].ToString();
        if (!string.IsNullOrEmpty(role)) claims.Add(new(ClaimTypes.Role, role));

        var identity = new ClaimsIdentity(claims, SchemeName);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
