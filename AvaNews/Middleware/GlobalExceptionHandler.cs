using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace AvaNews.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _pds;

    public GlobalExceptionHandler(IProblemDetailsService pds, ILogger<GlobalExceptionHandler> logger)
    {
        _pds = pds;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext http, Exception ex, CancellationToken ct)
    {
        var (status, title) = ex switch
        {
            OperationCanceledException when http.RequestAborted.IsCancellationRequested
                => (StatusCodes.Status499ClientClosedRequest, "Request was cancelled"),
            ValidationException
                => (StatusCodes.Status400BadRequest, "Validation failed"),
            ArgumentException
                => (StatusCodes.Status400BadRequest, "Bad request"),
            UnauthorizedAccessException
                => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            KeyNotFoundException
                => (StatusCodes.Status404NotFound, "Not found"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        if (status == StatusCodes.Status500InternalServerError)
            _logger.LogError(ex, "Unhandled exception");

        http.Response.StatusCode = status;
        await _pds.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = http,
            ProblemDetails =
            {
                Title = title,
                Status = status,
                Detail = ex.Message,
                Instance = http.Request.Path,
                Extensions = { ["traceId"] = http.TraceIdentifier }
            }
        });
        return true;
    }
}