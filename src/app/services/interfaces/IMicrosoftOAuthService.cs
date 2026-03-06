using backend.app.models.other;

namespace backend.app.services.interfaces
{
    public interface IMicrosoftOAuthService
    {
        Task<OAuthUser> VerifyTokenAsync(string microsoftToken);
    }
}
