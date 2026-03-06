using System.ComponentModel.DataAnnotations;
using backend.app.attributes;

namespace backend.app.dtos.request.auth
{
    public class ChangePasswordRequest
    {
        [Required]
        [StrongPassword(MinimumLength = 8)]
        [StringLength(30)]
        public required string Password{ get; set; }
    }
}