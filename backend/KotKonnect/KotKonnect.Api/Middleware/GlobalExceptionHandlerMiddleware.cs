namespace KotKonnect.Api.Middleware;

using System.Net;
using System.Text.Json;
using KotKonnect.Core.Exceptions;

// Traduit les exceptions des use cases en codes HTTP (endpoints fins).
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception interceptée : {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message),           // 404
            ForbiddenException => (HttpStatusCode.Forbidden, exception.Message),            // 403 (ownership)
            ArgumentException => (HttpStatusCode.BadRequest, exception.Message),            // 400
            InvalidOperationException => (HttpStatusCode.Conflict, exception.Message),      // 409 (email pris, déjà postulé)
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, exception.Message),// 401
            _ => (HttpStatusCode.InternalServerError, "Une erreur interne est survenue. Veuillez réessayer plus tard.")
        };

        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(JsonSerializer.Serialize(new { message }));
    }
}
