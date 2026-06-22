using System.Text.Json;

namespace FUNewsManagement.Web.Common;

public static class JsonOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true,
    };
}
