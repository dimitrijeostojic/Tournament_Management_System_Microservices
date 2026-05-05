using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AuthService.Middleware;

public sealed class GlobalExceptionHandlingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var errors = ex.Errors
                .Select(e => new { property = e.PropertyName, message = e.ErrorMessage })
                .ToList();

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                status = 400,
                title = "Validation failed",
                errors
            }));
        }
        catch (Exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            ProblemDetails problemDetails = new()
            {
                Status = context.Response.StatusCode,
                Type = "Server error",
                Title = "Server error",
                Detail = "An internal server has occured"
            };

            var json = JsonSerializer.Serialize(problemDetails);

            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(json);
        }
    }
}
