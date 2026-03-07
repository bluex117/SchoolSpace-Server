using System.IdentityModel.Tokens.Jwt;

using backend.app.configurations.environment;
using backend.app.errors.http;
using backend.app.http;
using backend.app.models.other;
using backend.app.services.interfaces;

using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace backend.app.services.implementations
{
    public class GoogleOAuthService : IGoogleOAuthService
    {
        private readonly string? _clientId;
        private readonly ConfigurationManager<OpenIdConnectConfiguration> _configManager;

        public GoogleOAuthService(IExternalApiClient apiClient)
        {
            _clientId = EnvironmentSetting.GoogleClientId;

            _configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                "https://accounts.google.com/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever(apiClient.HttpClient)
            );
        }

        public async Task<OAuthUser> VerifyTokenAsync(string googleToken)
        {
            if (_clientId == null)
                throw new NotAvaliableException("Google OAuth is not available");

            var oidcConfig = await _configManager.GetConfigurationAsync(CancellationToken.None);

            var validationParams = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = _clientId,
                ValidateIssuer = true,
                ValidIssuers = new[] { "https://accounts.google.com", "accounts.google.com" },
                IssuerSigningKeys = oidcConfig.SigningKeys,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2)
            };

            var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
            var principal = handler.ValidateToken(googleToken, validationParams, out _);

            var email =
                principal.Claims.FirstOrDefault(c => c.Type == "email")?.Value ??
                throw new UnauthorizedException("Missing Google email claim");

            var name =
                principal.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? email;

            var sub =
                principal.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ??
                throw new UnauthorizedException("Missing Google sub claim");

            return new OAuthUser(sub, email, name, "google");
        }
    }
}
