using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EVCharging.Filters;

/// <summary>
/// Translates common exceptions to standardized <see cref="ProblemDetails"/> responses.
/// </summary>
public class ApiExceptionFilter(ILogger<ApiExceptionFilter> logger) : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger = logger;

    public void OnException(ExceptionContext context)
    {
        var (statusCode, title) = context.Exception switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found."),
            InvalidOperationException => (StatusCodes.Status400BadRequest, "Invalid request."),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = context.Exception.Message,
            Instance = context.HttpContext.Request.Path
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            _logger.LogError(context.Exception, "Unhandled exception encountered.");
        else
            _logger.LogWarning(context.Exception, "Handled exception translated to HTTP {StatusCode}.", statusCode);

        context.Result = new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
        context.ExceptionHandled = true;
    }
}