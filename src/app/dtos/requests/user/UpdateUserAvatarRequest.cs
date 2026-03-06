using System.ComponentModel.DataAnnotations;

namespace backend.app.dtos.request.user
{
    public class UpdateUserAvatarRequest
    {
        [Required]
        [Url]
        [StringLength(2000)]
        public required string AvatarUrl { get; set; }
    }
}
