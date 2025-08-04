using System.Net;
using System.Text.Json;
using Domain.Exceptions;

namespace Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    #region DI
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    #endregion


    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context); // пробрасываем в следующий middleware
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            var (statusCode, message) = ex switch
            {
                WeatherNotFoundException => (HttpStatusCode.NotFound, ex.Message),
                _ => (HttpStatusCode.InternalServerError, "Weather service error")
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new
            {
                error = message,
                status = (int)statusCode
            });

            await context.Response.WriteAsync(result);
        }
    }



}