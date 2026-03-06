namespace backend.app.services.interfaces
{
    public interface ICaptchaService
    {
        Task<bool> VerifyCaptchaAsync(string token, CancellationToken cancellationToken = default);
    }
}