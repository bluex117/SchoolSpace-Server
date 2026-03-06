using System.ComponentModel.DataAnnotations;

namespace backend.app.dtos.request.auth
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}