using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using backend.app.configurations.environment;
using backend.app.models.core;
using backend.app.services.interfaces;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace backend.app.services.implementations
{
    public sealed class TokenService : ITokenService
    {
        private const string Issuer = "SchoolSpace";
        private const string Audience = "SchoolSpaceConsumers";
        private const string RefreshTokenKeyPrefix = "refresh_token:";
        private const string VerificationTokenKeyPrefix = "verification_token:";
        private const string VerificationEmailKeyPrefix = "verification_email:";
        private static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);
        private static readonly TimeSpan VerificationTokenLifetime = TimeSpan.FromHours(24);

        private readonly ICacheService _cache;
        private readonly SymmetricSecurityKey _signingKey;

        public TokenService(ICacheService cache)
        {
            _cache = cache;
            _signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(EnvironmentSetting.JwtSecretKeyAccess));
        }

        public string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Email),
                new(ClaimTypes.Role, user.Usertype.ToString())
            };

            var creds = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.Add(AccessTokenLifetime);

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateRefreshToken(int userId)
        {
            var tokenBytes = new byte[64];
            RandomNumberGenerator.Fill(tokenBytes);
            var token = Convert.ToBase64String(tokenBytes);

            var key = RefreshTokenKeyPrefix + token;
            await _cache.SetValueAsync(key, userId.ToString(), RefreshTokenLifetime);

            return token;
        }

        public async Task<int?> ValidateRefreshToken(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return null;

            var key = RefreshTokenKeyPrefix + refreshToken;
            var value = await _cache.GetValueAsync(key);
            if (value is null)
                return null;

            return int.TryParse(value, out var userId) ? userId : null;
        }

        public async Task<(string NewRefreshToken, int UserId)?> RotateRefreshTokenAsync(string refreshToken)
        {
            var userId = await ValidateRefreshToken(refreshToken);
            if (userId is null)
                return null;

            var key = RefreshTokenKeyPrefix + refreshToken;
            await _cache.DeleteKeyAsync(key);

            var newToken = await GenerateRefreshToken(userId.Value);
            return (newToken, userId.Value);
        }

        public async Task<bool> InvalidateRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return false;

            var key = RefreshTokenKeyPrefix + refreshToken;
            return await _cache.DeleteKeyAsync(key);
        }

        public async Task<string> GenerateVerificationToken(User user)
        {
            var tokenBytes = new byte[48];
            RandomNumberGenerator.Fill(tokenBytes);
            var token = Convert.ToBase64String(tokenBytes);

            var tokenKey = VerificationTokenKeyPrefix + token;
            var emailKey = VerificationEmailKeyPrefix + user.Email;

            await _cache.SetValueAsync(tokenKey, user.Id.ToString(), VerificationTokenLifetime);
            await _cache.SetValueAsync(emailKey, token, VerificationTokenLifetime);

            return token;
        }

        public async Task<int?> VerifyVerificationToken(string verifyToken)
        {
            if (string.IsNullOrWhiteSpace(verifyToken))
                return null;

            var key = VerificationTokenKeyPrefix + verifyToken;
            var value = await _cache.GetValueAsync(key);
            if (value is null)
                return null;

            return int.TryParse(value, out var userId) ? userId : null;
        }

        public async Task<string?> VerificationTokenExist(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var key = VerificationEmailKeyPrefix + email;
            return await _cache.GetValueAsync(key);
        }
    }
}
