using backend.app.configurations.environment;
using backend.app.errors.http;
using backend.app.models.other;
using backend.app.services.interfaces;

using Google.Apis.Auth;

using Polly;

namespace backend.app.services.implementations
{
    public class GoogleOAuthService : IGoogleOAuthService
    {
        private readonly string? _clientId;
        private static readonly ThreadLocal<Random> _jitter = new(() => new Random());

        private static readonly AsyncPolicy _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt =>
                {
                    double baseDelayMs = 200 * Math.Pow(2, attempt);
                    double jitter = 0.5 + _jitter.Value!.NextDouble();
                    return TimeSpan.FromMilliseconds(baseDelayMs * jitter);
                });

        public GoogleOAuthService()
        {
            _clientId = EnvironmentSetting.GoogleClientId;
        }

        public async Task<OAuthUser> VerifyTokenAsync(string googleToken)
        {
            if (_clientId == null)
                throw new NotAvaliableException("Google OAuth is not available");

            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _clientId }
            };

            var payload = await _retryPolicy.ExecuteAsync(() =>
                GoogleJsonWebSignature.ValidateAsync(googleToken, settings));

            return new OAuthUser(
                payload.Subject,
                payload.Email,
                payload.Name ?? payload.Email,
                "google"
            );
        }
    }
}
