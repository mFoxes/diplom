using Serilog.Context;
using Serilog.Core.Enrichers;

namespace GrandmaApi.Middlewares;

public class PushCorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public PushCorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        using (LogContext.Push(new PropertyEnricher("CorrelationId", Guid.NewGuid())))
        {
            await _next(httpContext);
        }
    }
}