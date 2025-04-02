using System.Text.Json;
using ChatService.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.API.Middlewares;

public class GlobalExceptionHandlingMiddleware(
    ILogger<GlobalExceptionHandlingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var statusCode = ex switch
            {
                BadRequestException => StatusCodes.Status400BadRequest,
                AlreadyExistsException => StatusCodes.Status400BadRequest,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                ForbiddenException => StatusCodes.Status403Forbidden,
                NotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };
            
            logger.LogError(
                ex,
                "HTTP {Method} {Path} => {StatusCode} | Error: {ErrorType}. Error message: {ErrorMessage}",
                context.Request.Method,
                context.Request.Path,
                statusCode,
                ex.GetType().Name,
                ex.Message);

            var details = new ProblemDetails
            {
                Title = statusCode == StatusCodes.Status500InternalServerError 
                    ? "Internal Server Error" 
                    : ex.GetType().Name,
                Type = ex.GetType().Name,
                Status = statusCode,
                Detail = ex.Message,
                Instance = context.Request.Path
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            
            var json = JsonSerializer.Serialize(details);

            await context.Response.WriteAsync(json);
        }
    }
}