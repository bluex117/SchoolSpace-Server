using backend.app.errors.http;
using backend.app.models.other;
using backend.app.services.interfaces;

namespace backend.main.Services
{
    public class OAuthService : IOAuthService
    {
        private readonly IGoogleOAuthService _googleOAuthService;
        private readonly IMicrosoftOAuthService _microsoftOAuthService;

        public OAuthService(
            IGoogleOAuthService googleOAuthService,
            IMicrosoftOAuthService microsoftOAuthService)
        {
            _googleOAuthService = googleOAuthService;
            _microsoftOAuthService = microsoftOAuthService;
        }

        public Task<OAuthUser> VerifyAppleTokenAsync(string appleToken)
        {
            throw new backend.app.errors.http.NotImplementedException();
        }

        public Task<OAuthUser> VerifyGoogleTokenAsync(string googleToken)
            => _googleOAuthService.VerifyTokenAsync(googleToken);

        public Task<OAuthUser> VerifyMicrosoftTokenAsync(string microsoftToken)
            => _microsoftOAuthService.VerifyTokenAsync(microsoftToken);
    }
}
