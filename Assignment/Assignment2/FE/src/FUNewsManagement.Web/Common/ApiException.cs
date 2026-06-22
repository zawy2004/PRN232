using System.Net;

namespace FUNewsManagement.Web.Common;

/// <summary>Raised when the BE API returns a non-success status; carries the BE's { "error": "..." } message.</summary>
public class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public ApiException(HttpStatusCode statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}
