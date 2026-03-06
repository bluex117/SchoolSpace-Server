namespace backend.app.dtos.responses.auth
{
    public class AuthResult
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public int UserId { get; set; }
        public required string Username { get; set; }
        public required string Role { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
