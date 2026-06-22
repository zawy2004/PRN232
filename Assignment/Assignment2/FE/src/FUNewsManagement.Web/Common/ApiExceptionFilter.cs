using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace FUNewsManagement.Web.Common;

/// <summary>
/// Catches ApiException thrown by the typed API clients so a BE error never surfaces as an
/// unhandled-exception page. A 401 means the JWT embedded in the FE auth cookie expired (or was
/// otherwise rejected) while the FE cookie session was still active -> force a clean re-login
/// instead of crashing. Everything else becomes a friendly TempData error + redirect back.
/// </summary>
public class ApiExceptionFilter : IAsyncExceptionFilter
{
    public async Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is not ApiException apiException)
            return;

        if (apiException.StatusCode == HttpStatusCode.Unauthorized)
        {
            var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl, sessionExpired = true });
            context.ExceptionHandled = true;
            return;
        }

        if (apiException.StatusCode == HttpStatusCode.Forbidden)
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            context.ExceptionHandled = true;
            return;
        }

        var tempDataFactory = context.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
        var tempData = tempDataFactory.GetTempData(context.HttpContext);
        tempData["Error"] = apiException.Message;

        var referer = context.HttpContext.Request.Headers.Referer.ToString();
        context.Result = string.IsNullOrEmpty(referer)
            ? new RedirectToActionResult("Index", "Home", null)
            : new RedirectResult(referer);
        context.ExceptionHandled = true;
    }
}
