using System.ComponentModel.DataAnnotations;

namespace backend.app.dtos.request.user
{
    public class UpdateUserRequest
    {
        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(100)]
        public string? Username { get; set; }

        [StringLength(30)]
        [Phone]
        public string? Phone { get; set; }
    }
}
