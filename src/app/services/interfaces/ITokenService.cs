using backend.app.models.core;

namespace backend.app.services.interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        Task<string> GenerateRefreshToken(int userId);
        Task<string> GenerateVerificationToken(User user);
        Task<int?> VerifyVerificationToken(string verifyToken);
        Task<int?> ValidateRefreshToken(string refreshToken);
        Task<(string NewRefreshToken, int UserId)?> RotateRefreshTokenAsync(string refreshToken);
        Task<bool> InvalidateRefreshTokenAsync(string refreshToken);
        Task<string?> VerificationTokenExist(string email);
    }
}
