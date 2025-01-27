using System.Text;

namespace WebApi.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;
        logger.LogInformation($"Request: {request.Method} {request.Path}");
        logger.LogInformation($"Headers: {request.Headers}");

        context.Request.EnableBuffering();
        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;
        
        logger.LogInformation($"Request Body: {body}");

        await next(context);
        
        var response = context.Response;
        logger.LogInformation("Response: {StatusCode}", response.StatusCode);
    }
}