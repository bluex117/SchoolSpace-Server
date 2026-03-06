using backend.app.models.core;

namespace backend.app.dtos.responses.general
{
    public class UserResponse
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required UserRole Usertype { get; set; }
        public required UserStatus Status { get; set; }
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
