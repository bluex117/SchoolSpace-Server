using System.ComponentModel.DataAnnotations;

namespace backend.app.dtos.request.auth
{
    public class SignupRequest : AuthRequest
    {
        [Required]
        [RegularExpression("^(student|teacher|assistant)$",
        ErrorMessage = "Role must be 'student', 'teacher' or 'assistant'.")]
        public required string Role { get; set; }
    } 
}