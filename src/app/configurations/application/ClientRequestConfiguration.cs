using System.Security.Claims;

using backend.app.dtos.general;
using backend.app.configurations.security;

namespace backend.app.configurations.application
{
    public static class ClientRequestConfiguration
    {
        public static IServiceCollection AddClientRequestInspection(this IServiceCollection services)
        {
            services.AddScoped<ClientRequestInfo>();
            return services;
        }

        public static IApplicationBuilder UseClientRequestInspection(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ClientRequestInspectionMiddleware>();
        }
    }

    public class ClientRequestInspectionMiddleware
    {
        private readonly RequestDelegate _next;

        public ClientRequestInspectionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ClientRequestInfo requestInfo)
        {
            requestInfo.IpAddress = ResolveIpAddress(context);
            requestInfo.ClientName = ResolveClientName(context);
            requestInfo.DeviceType = ResolveDeviceType(context);

            await _next(context);

            if (context.User.Identity?.IsAuthenticated == true)
            {
                requestInfo.UserPayload = ResolveUserPayload(context.User);
            }
        }

        private static string ResolveIpAddress(HttpContext context)
        {
            var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwarded))
            {
                return forwarded.Split(',', StringSplitOptions.TrimEntries)[0];
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        private static string ResolveClientName(HttpContext context)
        {
            var userAgent = context.Request.Headers.UserAgent.ToString();
            if (string.IsNullOrWhiteSpace(userAgent))
                return "Unknown";

            if (userAgent.Contains("Edg/", StringComparison.OrdinalIgnoreCase)) return "Edge";
            if (userAgent.Contains("OPR/", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("Opera", StringComparison.OrdinalIgnoreCase)) return "Opera";
            if (userAgent.Contains("Chrome/", StringComparison.OrdinalIgnoreCase)) return "Chrome";
            if (userAgent.Contains("Safari/", StringComparison.OrdinalIgnoreCase) &&
                !userAgent.Contains("Chrome", StringComparison.OrdinalIgnoreCase)) return "Safari";
            if (userAgent.Contains("Firefox/", StringComparison.OrdinalIgnoreCase)) return "Firefox";

            if (userAgent.Contains("PostmanRuntime", StringComparison.OrdinalIgnoreCase)) return "Postman";
            if (userAgent.Contains("curl/", StringComparison.OrdinalIgnoreCase)) return "cURL";
            if (userAgent.Contains("axios/", StringComparison.OrdinalIgnoreCase)) return "Axios";
            if (userAgent.Contains("HttpClient", StringComparison.OrdinalIgnoreCase)) return "HttpClient";

            return userAgent.Split('/')[0].Trim();
        }

        private static string ResolveDeviceType(HttpContext context)
        {
            var userAgent = context.Request.Headers.UserAgent.ToString();
            if (string.IsNullOrWhiteSpace(userAgent))
                return "Unknown";

            if (userAgent.Contains("Mobi", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase) &&
                !userAgent.Contains("Tablet", StringComparison.OrdinalIgnoreCase))
                return "Mobile";

            if (userAgent.Contains("Tablet", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase))
                return "Tablet";

            if (userAgent.Contains("Windows", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("Macintosh", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("Linux", StringComparison.OrdinalIgnoreCase) &&
                !userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase))
                return "Desktop";

            if (userAgent.Contains("PostmanRuntime", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("curl/", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("HttpClient", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("axios/", StringComparison.OrdinalIgnoreCase))
                return "API Client";

            return "Unknown";
        }

        private static UserIdentityPayload? ResolveUserPayload(ClaimsPrincipal user)
        {
            string? idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? emailClaim = user.FindFirst(ClaimTypes.Name)?.Value;
            string? roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(idClaim) || string.IsNullOrEmpty(emailClaim) || string.IsNullOrEmpty(roleClaim))
                return null;

            if (!int.TryParse(idClaim, out int id))
                return null;

            return new UserIdentityPayload(id, emailClaim, roleClaim);
        }
    }
}
