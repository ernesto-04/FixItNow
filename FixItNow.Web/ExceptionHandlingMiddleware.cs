using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace FixItNow.Web;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IHostEnvironment env)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (statusCode, title) = ex switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found."),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized."),
            ArgumentException => (HttpStatusCode.BadRequest, "Bad request."),
            InvalidOperationException => (HttpStatusCode.UnprocessableEntity, "Operation not valid."),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        logger.LogError(
            ex,
            "Unhandled exception on {Method} {Path} — {StatusCode} {Title}",
            context.Request.Method,
            context.Request.Path,
            (int)statusCode,
            title);

        var problem = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Instance = context.Request.Path,
            Detail = env.IsDevelopment() ? ex.ToString() : null,
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions));
    }
}