using System.ComponentModel.DataAnnotations;
using backend.app.attributes.validation;

namespace backend.app.dtos.request.auth
{
    public class SignupRequest : AuthRequest
    {
        [Required]
        [ValidRole]
        public required string Role { get; set; }
    } 
}