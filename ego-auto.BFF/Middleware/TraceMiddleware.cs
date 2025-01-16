using ego_auto.BFF.Application.Contracts.Application;
using ego_auto.BFF.Domain.Entities;
using ego_auto.BFF.Utilities;
using System.Text.Json;

namespace ego_auto.BFF.Middleware;

public class TraceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly List<string> _authFreePaths;

    public TraceMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));

        // Define paths that don't require authentication
        _authFreePaths = new List<string>
        {
            "/api/user/log-in",
            //"/api/user/sign-up"
            // Add other public paths here
        };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Check if the current path is in the authentication-free list
            var requestPath = context.Request.Path.Value?.ToLowerInvariant();
            if (!_authFreePaths.Contains(requestPath))
            {
                // For authenticated endpoints, set the session user
                await TraceHelper.SetSessionUser(context);
            }

            // Proceed to the next middleware or controller
            await _next(context);
        }
        catch (Exception ex)
        {
            // Handle exceptions and generate a response
            var (statusCode, responseObj) = TraceHelper.HandleExceptions(ex);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var responseJson = JsonSerializer.Serialize(responseObj);
            await context.Response.WriteAsync(responseJson);

            // Optionally log the exception
            // Example: Logger.LogError(ex, "Unhandled exception in TraceMiddleware");
        }
        finally
        {
            try
            {
                // Only reset session user if authentication was applied
                var requestPath = context.Request.Path.Value?.ToLowerInvariant();
                if (!_authFreePaths.Contains(requestPath))
                {
                    await TraceHelper.ResetSessionUser(context);
                }
            }
            catch (Exception resetEx)
            {
                // Log cleanup errors, but don't throw
                // Example: Logger.LogWarning(resetEx, "Error during ResetSessionUser in TraceMiddleware");
            }
        }
    }
}
