using System.ComponentModel.DataAnnotations;

namespace backend.app.dtos.request.auth
{
    public class RefreshTokenRequest
    {
        [Required]
        public required string RefreshToken { get; set; }
    }
}
