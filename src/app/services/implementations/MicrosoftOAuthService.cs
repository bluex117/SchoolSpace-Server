using System.IdentityModel.Tokens.Jwt;

using backend.app.configurations.environment;
using backend.app.errors.http;
using backend.app.models.other;
using backend.app.services.interfaces;

using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace backend.app.services.implementations
{
    public class MicrosoftOAuthService : IMicrosoftOAuthService
    {
        private readonly string? _clientId;

        public MicrosoftOAuthService()
        {
            _clientId = EnvironmentSetting.MicrosoftClientId;
        }

        public async Task<OAuthUser> VerifyTokenAsync(string microsoftToken)
        {
            if (_clientId == null)
                throw new NotAvaliableException("Microsoft OAuth is not available");

            var authority = "https://login.microsoftonline.com/common/v2.0";

            var configManager =
                new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"{authority}/.well-known/openid-configuration",
                    new OpenIdConnectConfigurationRetriever()
                );

            var validationParams = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateLifetime = true,
                RequireSignedTokens = true,

                ValidAudience = _clientId,
                ConfigurationManager = configManager,

                ValidateIssuer = true,
                IssuerValidator = (issuer, token, parameters) =>
                {
                    if (issuer.StartsWith("https://login.microsoftonline.com/")
                        && issuer.EndsWith("/v2.0"))
                    {
                        return issuer;
                    }

                    throw new SecurityTokenInvalidIssuerException(
                        $"Invalid issuer: {issuer}");
                },

                ClockSkew = TimeSpan.FromMinutes(2)
            };

            var handler = new JwtSecurityTokenHandler
            {
                MapInboundClaims = false
            };

            var principal = handler.ValidateToken(
                microsoftToken,
                validationParams,
                out _
            );

            var email =
                principal.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value ??
                principal.Claims.FirstOrDefault(c => c.Type == "email")?.Value ??
                throw new UnauthorizedException("Missing Microsoft email claim");

            var name =
                principal.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? email;

            var sub =
                principal.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ??
                throw new UnauthorizedException("Missing Microsoft sub claim");

            return await Task.FromResult(new OAuthUser(sub, email, name, "microsoft"));
        }
    }
}
