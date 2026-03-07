using backend.app.configurations.environment;

namespace backend.app.configurations.security
{
    public static class SecurityHeadersConfiguration
    {
        private const string XContentTypeOptions = "X-Content-Type-Options";
        private const string XFrameOptions = "X-Frame-Options";
        private const string XPermittedCrossDomain = "X-Permitted-Cross-Domain-Policies";
        private const string ReferrerPolicy = "Referrer-Policy";
        private const string PermissionsPolicy = "Permissions-Policy";
        private const string ContentSecurityPolicy = "Content-Security-Policy";
        private const string CrossOriginOpenerPolicy = "Cross-Origin-Opener-Policy";
        private const string CrossOriginResourcePolicy = "Cross-Origin-Resource-Policy";
        private const string XDnsPrefetchControl = "X-DNS-Prefetch-Control";
        private const string CacheControl = "Cache-Control";

        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            return app.Use(async (context, next) =>
            {
                context.Response.Headers[XContentTypeOptions] = "nosniff";

                context.Response.Headers[XFrameOptions] = "DENY";

                context.Response.Headers[XPermittedCrossDomain] = "none";

                context.Response.Headers[ReferrerPolicy] = "strict-origin-when-cross-origin";

                context.Response.Headers[PermissionsPolicy] = "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()";

                context.Response.Headers[ContentSecurityPolicy] = "default-src 'none'; frame-ancestors 'none'";

                context.Response.Headers[CrossOriginOpenerPolicy] = "same-origin";

                context.Response.Headers[CrossOriginResourcePolicy] = "same-origin";

                context.Response.Headers[XDnsPrefetchControl] = "off";

                context.Response.Headers[CacheControl] = "no-store";

                await next(context);
            });
        }

        public static IApplicationBuilder UseHttpsEnforcement(this IApplicationBuilder app)
        {
            var env = EnvironmentSetting.AppEnvironment;
            if (env is "development" or "test")
                return app;

            app.UseHsts();
            app.UseHttpsRedirection();
            return app;
        }
    }
}
