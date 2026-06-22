using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace FUNewsManagement.Web.Common;

/// <summary>
/// No JWT for now: attaches the signed-in user's identity (from the FE's own cookie login) as
/// plain headers on every API call to the BE. The BE's HeaderIdentityAuthenticationHandler trusts
/// these headers as-is - there is no signature, so this is only safe because the FE is the BE's
/// sole caller in this setup. Header names must match HeaderIdentityAuthenticationHandler in the BE.
/// </summary>
public class IdentityForwardingHandler : DelegatingHandler
{
    public const string AccountIdHeader = "X-Account-Id";
    public const string AccountNameHeader = "X-Account-Name";
    public const string AccountEmailHeader = "X-Account-Email";
    public const string AccountRoleHeader = "X-Account-Role";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdentityForwardingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var accountId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(accountId))
        {
            request.Headers.Add(AccountIdHeader, accountId);
            AddIfPresent(request, AccountNameHeader, user!.FindFirst(ClaimTypes.Name)?.Value);
            AddIfPresent(request, AccountEmailHeader, user.FindFirst(ClaimTypes.Email)?.Value);
            AddIfPresent(request, AccountRoleHeader, user.FindFirst(ClaimTypes.Role)?.Value);
        }

        return base.SendAsync(request, cancellationToken);
    }

    private static void AddIfPresent(HttpRequestMessage request, string header, string? value)
    {
        if (!string.IsNullOrEmpty(value)) request.Headers.Add(header, value);
    }
}
