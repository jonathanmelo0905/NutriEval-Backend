using System.Security.Claims;

namespace NutriEval.API.Middleware;

public class TenantMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var claim = context.User.FindFirst(ClaimTypes.NameIdentifier)
                     ?? context.User.FindFirst("sub");

            if (claim is not null && Guid.TryParse(claim.Value, out var tenantId))
                context.Items["TenantId"] = tenantId;
        }

        await next(context);
    }
}

public static class TenantMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantContext(this IApplicationBuilder app) =>
        app.UseMiddleware<TenantMiddleware>();
}
