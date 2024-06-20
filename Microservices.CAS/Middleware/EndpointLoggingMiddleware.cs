using Serilog;

namespace Microservices.CAS.Middleware;

public class EndpointLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public EndpointLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // Log request
        var request = await FormatRequest(context.Request);
        Log.Information("Incoming Request: {Request}", request);

        // Call the next delegate/middleware in the pipeline
        await _next(context);

        // Log response
        var response = await FormatResponse(context.Response);
        Log.Information("Outgoing Response: {Response}", response);
    }

    private async Task<string> FormatRequest(HttpRequest request)
    {
        if (!request.HasFormContentType &&!string.IsNullOrEmpty(request.ContentType) && request.ContentType.ToLower().Contains("text"))
        {
            request.EnableBuffering();
            var body = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Position = 0;
            return $"{request.Method} {request.Path} {request.QueryString} {body}";
        }
        else
        {
            return $"{request.Method} {request.Path} {request.QueryString}";
        }
    }

    private async Task<string> FormatResponse(HttpResponse response)
    {
        if (response.Body.CanSeek)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return $"{response.StatusCode}: {text}";
        }
        else
        {
            return $"{response.StatusCode}";
        }
    }
}
