using backend.app.configurations.resources;
using backend.app.repositories.attributes;
using backend.app.repositories.extensions;
using backend.app.repositories.implementations;
using backend.app.repositories.interfaces;
using backend.app.repositories.resilience;
using backend.app.services.implementations;
using backend.app.services.interfaces;
using backend.app.utilities.implementation;
using backend.app.utilities.interfaces;
using backend.main.Services;

namespace backend.app.configurations.application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<ICacheService, ReconnectableCacheService>();
            services.AddTransient<ICaptchaService, GoogleCaptchaService>();
            services.AddTransient<IGoogleOAuthService, GoogleOAuthService>();
            services.AddTransient<IMicrosoftOAuthService, MicrosoftOAuthService>();
            services.AddTransient<IOAuthService, OAuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();

            services.AddSingleton<IRepositoryResiliencePolicy, RepositoryResiliencePolicy>();
            services.AddSingleton<IRepositoryAttributeResolver, RepositoryAttributeResolver>();
            services.AddRepositoryWithProxy<IUserRepository, UserRepository>();
            services.AddRepositoryWithProxy<IContactRepository, ContactRepository>();
            services.AddRepositoryWithProxy<IReportRepository, ReportRepository>();

            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<IReportService, ReportService>();

            services.AddSingleton<ICustomLogger, FileLogger>();

            return services;
        }
    }
}