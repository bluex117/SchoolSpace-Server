using backend.app.dtos.request.auth;
using backend.app.dtos.responses.auth;

namespace backend.app.services.interfaces
{
    public interface IAuthService
    {
        Task<AuthResult?> LoginAsync(string email, string password, bool rememberMe = false);
        Task<AuthResult?> SignupAsync(SignupRequest request);
        Task<bool> VerifyAsync(string verificationToken);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ChangePasswordAsync(string verificationToken, string newPassword);
        Task<AuthResult?> RefreshAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshToken);
        Task<AuthResult?> LoginWithGoogleAsync(string googleToken);
        Task<AuthResult?> LoginWithMicrosoftAsync(string microsoftToken);
    }
}
