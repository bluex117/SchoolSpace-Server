namespace backend.app.dtos.request.auth
{
    public class LoginRequest : AuthRequest
    {
        public new bool RememberMe { get; set; } = false;
    }
}