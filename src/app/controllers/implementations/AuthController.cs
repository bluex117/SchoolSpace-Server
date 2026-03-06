using backend.app.configurations.security;
using backend.app.dtos.request.auth;
using backend.app.dtos.responses.auth;
using backend.app.errors.http;
using backend.app.services.interfaces;
using backend.app.utilities.implementation;
using backend.app.utilities.interfaces;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace backend.app.implementations.Controllers
{
    [ApiController]
    [Route("auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IAntiforgery _antiforgery;
        private readonly ICustomLogger _logger;

        public AuthController(IAuthService authService, IAntiforgery antiforgery, ICustomLogger logger)
        {
            _authService = authService;
            _antiforgery = antiforgery;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.LoginAsync(request.Email, request.Password, request.RememberMe);
                CsrfConfiguration.SetCsrfCookie(HttpContext, _antiforgery);
                return Ok(result);
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                _logger.Error($"[AuthController] Login failed: {e}");
                return HandleError.Resolve(e);
            }
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.SignupAsync(request);
                CsrfConfiguration.SetCsrfCookie(HttpContext, _antiforgery);
                return CreatedAtAction(nameof(Login), result);
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                _logger.Error($"[AuthController] Signup failed: {e}");
                return HandleError.Resolve(e);
            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromQuery] string token, CancellationToken cancellationToken)
        {
            try
            {
                await _authService.VerifyAsync(token);
                return Ok(new { message = "Account verified successfully." });
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                _logger.Error($"[AuthController] Verify failed: {e}");
                return HandleError.Resolve(e);
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _authService.ForgotPasswordAsync(request.Email);
                return Ok(new { message = "If the email exists, a reset link has been sent." });
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                _logger.Error($"[AuthController] ForgotPassword failed: {e}");
                return HandleError.Resolve(e);
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromQuery] string token, [FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _authService.ChangePasswordAsync(token, request.Password);
                return Ok(new { message = "Password changed successfully." });
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                _logger.Error($"[AuthController] ChangePassword failed: {e}");
                return HandleError.Resolve(e);
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
        {
            try
            {
                string? refreshToken = Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                    throw new UnauthorizedException("Missing refresh token");
                var result = await _authService.RefreshAsync(refreshToken);
                CsrfConfiguration.SetCsrfCookie(HttpContext, _antiforgery);
                return Ok(result);
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                _logger.Error($"[AuthController] Refresh failed: {e}");
                return HandleError.Resolve(e);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            try
            {
                string? refreshToken = Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                    throw new UnauthorizedException("Missing refresh token");
                var success = await _authService.LogoutAsync(refreshToken);
                return Ok(new { message = success ? "Logged out successfully." : "Token was already invalid or missing." });
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                _logger.Error($"[AuthController] Logout failed: {e}");
                return HandleError.Resolve(e);
            }
        }

        [HttpGet("csrf")]
        public IActionResult Csrf()
        {
            try
            {
                var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
                return Ok(new
                {
                    token = tokens.RequestToken
                });
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                _logger.Error($"[AuthController] Csrf failed: {e}");
                return HandleError.Resolve(e);
            }
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleOAuth([FromBody] GoogleOAuthRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.LoginWithGoogleAsync(request.Token);
                CsrfConfiguration.SetCsrfCookie(HttpContext, _antiforgery);
                return Ok(result);
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                _logger.Error($"[AuthController] GoogleOAuth failed: {e}");
                return HandleError.Resolve(e);
            }
        }

        [HttpPost("microsoft")]
        public async Task<IActionResult> MicrosoftOAuth([FromBody] MicrosoftOAuthRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.LoginWithMicrosoftAsync(request.Token);
                CsrfConfiguration.SetCsrfCookie(HttpContext, _antiforgery);
                return Ok(result);
            }
            catch (Exception e)
            {
                if (e is AppException)
                    return HandleError.Resolve(e);

                _logger.Error($"[AuthController] MicrosoftOAuth failed: {e}");
                return HandleError.Resolve(e);
            }
        }
    }
}
