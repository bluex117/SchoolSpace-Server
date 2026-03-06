using backend.app.models.other;

namespace backend.app.services.interfaces
{
    public interface IGoogleOAuthService
    {
        Task<OAuthUser> VerifyTokenAsync(string googleToken);
    }
}
