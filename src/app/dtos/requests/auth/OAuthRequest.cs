using System.ComponentModel.DataAnnotations;

namespace backend.app.dtos.request.auth
{
    public abstract class OAuthRequest
    {
        [Required]
        public required string Token { get; set; }
    }
}