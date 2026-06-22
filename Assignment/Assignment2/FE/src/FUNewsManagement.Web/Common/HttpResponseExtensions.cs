using System.Text.Json;

namespace FUNewsManagement.Web.Common;

public static class HttpResponseExtensions
{
    public static async Task EnsureApiSuccessAsync(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;

        var message = $"Request failed with status {(int)response.StatusCode}.";
        try
        {
            var body = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(body))
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("error", out var errorProp))
                    message = errorProp.GetString() ?? message;
            }
        }
        catch (JsonException)
        {
            // Body wasn't JSON; fall back to the generic status-code message.
        }

        throw new ApiException(response.StatusCode, message);
    }
}
