using System.ComponentModel.DataAnnotations;
using backend.app.attributes.validation;

namespace backend.app.dtos.request.auth
{
    public abstract class AuthRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StrongPassword(MinimumLength = 8)]
        [StringLength(30)]
        public required string Password { get; set; }

        [Required] public required string Captcha { get; set; }
        public bool RememberMe { get; set; } = false;

    }
}