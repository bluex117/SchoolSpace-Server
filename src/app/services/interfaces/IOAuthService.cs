using backend.app.models.other;

namespace backend.app.services.interfaces
{
    public interface IOAuthService
    {
        Task<OAuthUser> VerifyGoogleTokenAsync(string googleToken);
        Task<OAuthUser> VerifyMicrosoftTokenAsync(string microsoftToken);
        Task<OAuthUser> VerifyAppleTokenAsync(string appleToken);
    }
}