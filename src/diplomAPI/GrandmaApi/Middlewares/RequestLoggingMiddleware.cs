using System.Net;
using System.Security.Claims;
using MediatR;

namespace GrandmaApi.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
        finally
        {

            var user = (ClaimsIdentity)context.User.Identity;
            var username = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "undefined";
            var method = context.Request?.Method;
            var path = context.Request?.Path.Value;
            var statusCode = context.Response.StatusCode;
            var query = context.Request.QueryString;
            var logLevel = LogLevel.Information;
            if (statusCode == (int)HttpStatusCode.NotFound)
            {
                logLevel = LogLevel.Warning;
            }
            _logger.Log(logLevel, $"User {username} requested {method} {path}{query} => StatusCode {statusCode}");
        }
    }
}