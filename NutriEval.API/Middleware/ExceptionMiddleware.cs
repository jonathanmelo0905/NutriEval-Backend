using FluentValidation;
using NutriEval.API.Models;

namespace NutriEval.API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Excepción no controlada en {Method} {Path}: {Message}",
                context.Request.Method, context.Request.Path, ex.Message);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errors) = exception switch
        {
            ValidationException e => (
                StatusCodes.Status400BadRequest,
                "Error de validación",
                e.Errors.Select(f => f.ErrorMessage)),

            KeyNotFoundException e => (
                StatusCodes.Status404NotFound,
                e.Message,
                Enumerable.Empty<string>()),

            UnauthorizedAccessException e => (
                StatusCodes.Status401Unauthorized,
                e.Message,
                Enumerable.Empty<string>()),

            ArgumentException e => (
                StatusCodes.Status400BadRequest,
                e.Message,
                Enumerable.Empty<string>()),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Error interno del servidor",
                Enumerable.Empty<string>())
        };

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(
            ApiResponse.Fail(message, errors));
    }
}

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionMiddleware>();
}
