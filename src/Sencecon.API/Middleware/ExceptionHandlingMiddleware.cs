using System.Net;
using System.Text.Json;
using Sencecon.Domain.Exceptions;
using ValidationException = Sencecon.Application.Common.Exceptions.ValidationException;

namespace Sencecon.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        object payload;

        switch (exception)
        {
            case ValidationException validationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                payload = new
                {
                    title = "One or more validation errors occurred.",
                    status = context.Response.StatusCode,
                    errors = validationException.Errors
                };
                break;

            case NotFoundException notFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                payload = new
                {
                    title = notFoundException.Message,
                    status = context.Response.StatusCode
                };
                break;

            case ForbiddenAccessException forbiddenAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                payload = new
                {
                    title = forbiddenAccessException.Message,
                    status = context.Response.StatusCode
                };
                break;

            case ConflictException conflictException:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                payload = new
                {
                    title = conflictException.Message,
                    status = context.Response.StatusCode
                };
                break;

            case UnauthorizedAccessException unauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                payload = new
                {
                    title = unauthorizedAccessException.Message,
                    status = context.Response.StatusCode
                };
                break;

            default:
                _logger.LogError(exception, "An unhandled exception occurred");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                payload = new
                {
                    title = "An unexpected error occurred.",
                    status = context.Response.StatusCode
                };
                break;
        }

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
