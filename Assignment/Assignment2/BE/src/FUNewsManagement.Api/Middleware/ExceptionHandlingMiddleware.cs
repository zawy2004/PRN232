using FUNewsManagement.BusinessLogic.Exceptions;

namespace FUNewsManagement.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (EntityNotFoundException ex)
        {
            await WriteProblem(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (ForbiddenOperationException ex)
        {
            await WriteProblem(context, StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (BusinessRuleException ex)
        {
            await WriteProblem(context, StatusCodes.Status409Conflict, ex.Message);
        }
    }

    private static Task WriteProblem(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsJsonAsync(new { error = message });
    }
}
