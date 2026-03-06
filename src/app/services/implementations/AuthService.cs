using backend.app.dtos.request.auth;
using backend.app.dtos.responses.auth;
using backend.app.errors.app;
using backend.app.errors.http;
using backend.app.models.core;
using backend.app.repositories.interfaces;
using backend.app.services.interfaces;

namespace backend.app.services.implementations
{
    public sealed class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IOAuthService _oauthService;

        public AuthService(IUserRepository userRepository, ITokenService tokenService, IOAuthService oauthService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _oauthService = oauthService;
        }

        public async Task<AuthResult?> LoginAsync(string email, string password, bool rememberMe = false)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user is null || user.Password is null)
                throw new InvalidCredentialsException();

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                throw new InvalidCredentialsException();

            if (user.Status != UserStatus.Active)
                throw new UserNotActiveException();

            return await BuildAuthResultAsync(user);
        }

        public async Task<AuthResult?> SignupAsync(SignupRequest request)
        {
            if (await _userRepository.EmailExistsAsync(request.Email))
                throw new EmailAlreadyExistsException();

            var role = ParseRole(request.Role);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Email = request.Email,
                Password = hashedPassword,
                Usertype = role,
                Status = UserStatus.Active,
                Username = request.Email
            };

            user = await _userRepository.CreateUserAsync(user);

            return await BuildAuthResultAsync(user);
        }

        public async Task<bool> VerifyAsync(string verificationToken)
        {
            if (string.IsNullOrWhiteSpace(verificationToken))
                throw new InvalidOrExpiredVerificationTokenException();

            var userId = await _tokenService.VerifyVerificationToken(verificationToken);
            if (userId is null)
                throw new InvalidOrExpiredVerificationTokenException();

            var user = await _userRepository.GetUserAsync(userId.Value);
            if (user is null)
                throw new ResourceNotFoundException("User not found.");

            var updated = new User
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,
                Usertype = user.Usertype,
                Status = UserStatus.Active,
                Username = user.Username,
                Name = user.Name,
                AvatarUrl = user.AvatarUrl,
                Phone = user.Phone,
                MicrosoftId = user.MicrosoftId,
                GoogleId = user.GoogleId,
                CreatedAt = user.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            return await _userRepository.UpdatePartialAsync(updated) is not null;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user is null)
                return false;

            await _tokenService.GenerateVerificationToken(user);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(string verificationToken, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(verificationToken) || string.IsNullOrWhiteSpace(newPassword))
                throw new InvalidOrExpiredVerificationTokenException();

            var userId = await _tokenService.VerifyVerificationToken(verificationToken);
            if (userId is null)
                throw new InvalidOrExpiredVerificationTokenException();

            var user = await _userRepository.GetUserAsync(userId.Value);
            if (user is null)
                throw new ResourceNotFoundException("User not found.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            var updated = new User
            {
                Id = user.Id,
                Email = user.Email,
                Password = hashedPassword,
                Usertype = user.Usertype,
                Status = user.Status,
                Username = user.Username,
                Name = user.Name,
                AvatarUrl = user.AvatarUrl,
                Phone = user.Phone,
                MicrosoftId = user.MicrosoftId,
                GoogleId = user.GoogleId,
                CreatedAt = user.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            return await _userRepository.UpdatePartialAsync(updated) is not null;
        }

        public async Task<AuthResult?> RefreshAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new InvalidOrExpiredRefreshTokenException();

            var rotated = await _tokenService.RotateRefreshTokenAsync(refreshToken);
            if (rotated is null)
                throw new InvalidOrExpiredRefreshTokenException();

            var user = await _userRepository.GetUserAsync(rotated.Value.UserId);
            if (user is null)
                throw new ResourceNotFoundException("User not found.");
            if (user.Status != UserStatus.Active)
                throw new UserNotActiveException();

            var accessToken = _tokenService.GenerateAccessToken(user);
            return new AuthResult
            {
                AccessToken = accessToken,
                RefreshToken = rotated.Value.NewRefreshToken,
                UserId = user.Id,
                Username = user.Username ?? user.Email,
                Role = user.Usertype.ToString(),
                AvatarUrl = user.AvatarUrl
            };
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            return await _tokenService.InvalidateRefreshTokenAsync(refreshToken);
        }

        public async Task<AuthResult?> LoginWithGoogleAsync(string googleToken)
        {
            if (string.IsNullOrWhiteSpace(googleToken))
                throw new InvalidCredentialsException();

            var oauthUser = await _oauthService.VerifyGoogleTokenAsync(googleToken);
            var user = await _userRepository.GetUserByGoogleIdAsync(oauthUser.Id);

            if (user is null)
            {
                user = await _userRepository.GetUserByEmailAsync(oauthUser.Email);
                if (user is not null)
                    await _userRepository.UpdateProviderIdsAsync(user.Id, oauthUser.Id, null);
                else
                {
                    user = await _userRepository.CreateUserAsync(new User
                    {
                        Email = oauthUser.Email,
                        Username = oauthUser.Email,
                        Name = oauthUser.Name,
                        Password = null,
                        Usertype = UserRole.Student,
                        Status = UserStatus.Active,
                        GoogleId = oauthUser.Id
                    });
                }
            }

            if (user.Status != UserStatus.Active)
                throw new UserNotActiveException();

            Console.WriteLine(user.Email);
            return await BuildAuthResultAsync(user);
        }

        public async Task<AuthResult?> LoginWithMicrosoftAsync(string microsoftToken)
        {
            if (string.IsNullOrWhiteSpace(microsoftToken))
                throw new InvalidCredentialsException();

            var oauthUser = await _oauthService.VerifyMicrosoftTokenAsync(microsoftToken);
            var user = await _userRepository.GetUserByMicrosoftIdAsync(oauthUser.Id);

            if (user is null)
            {
                user = await _userRepository.GetUserByEmailAsync(oauthUser.Email);
                if (user is not null)
                    await _userRepository.UpdateProviderIdsAsync(user.Id, null, oauthUser.Id);
                else
                {
                    user = await _userRepository.CreateUserAsync(new User
                    {
                        Email = oauthUser.Email,
                        Username = oauthUser.Email,
                        Name = oauthUser.Name,
                        Password = null,
                        Usertype = UserRole.Student,
                        Status = UserStatus.Active,
                        MicrosoftId = oauthUser.Id
                    });
                }
            }

            if (user.Status != UserStatus.Active)
                throw new UserNotActiveException();

            return await BuildAuthResultAsync(user);
        }

        private async Task<AuthResult> BuildAuthResultAsync(User user)
        {
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = await _tokenService.GenerateRefreshToken(user.Id);

            return new AuthResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = user.Id,
                Username = user.Username ?? user.Email,
                Role = user.Usertype.ToString(),
                AvatarUrl = user.AvatarUrl
            };
        }

        private static UserRole ParseRole(string role)
        {
            return role.ToLowerInvariant() switch
            {
                "teacher" => UserRole.Teacher,
                "assistant" => UserRole.Admin,
                _ => UserRole.Student
            };
        }
    }
}
