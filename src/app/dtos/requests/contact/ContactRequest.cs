using System.ComponentModel.DataAnnotations;

namespace backend.app.dtos.request.contact
{
    public class ContactRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(256)]
        public required string Email { get; set; }

        [Required]
        [StringLength(200)]
        public required string Topic { get; set; }

        [Required]
        [StringLength(2000)]
        public required string Description { get; set; }
    }
}
