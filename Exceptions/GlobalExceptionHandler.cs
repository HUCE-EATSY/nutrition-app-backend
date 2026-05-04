using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace nutrition_app_backend.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var statusCode = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            ForbiddenException => StatusCodes.Status403Forbidden,
            BusinessException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var detail = _environment.IsDevelopment()
            ? exception.ToString()
            : statusCode == StatusCodes.Status500InternalServerError
                ? "Unexpected error occurred"
                : exception.Message;

        var problemDetails = new ProblemDetails
        {
            Title = "Request failed",
            Status = statusCode,
            Detail = detail,
            Instance = httpContext.Request.Path.ToString()
        };

        if (exception is BusinessException businessException)
        {
            problemDetails.Extensions["code"] = businessException.Code;
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
