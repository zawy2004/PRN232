using FUNewsManagement.BusinessLogic.Options;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace FUNewsManagement.BusinessLogic.Security;

public class GoogleTokenValidator : IGoogleTokenValidator
{
    private readonly GoogleAuthOptions _options;

    public GoogleTokenValidator(IOptions<GoogleAuthOptions> options)
    {
        _options = options.Value;
    }

    public async Task<GoogleIdentity?> ValidateAsync(string idToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _options.ClientId },
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            return payload.EmailVerified ? new GoogleIdentity(payload.Email, payload.Name) : null;
        }
        catch (InvalidJwtException)
        {
            return null;
        }
    }
}
