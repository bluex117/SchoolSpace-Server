namespace backend.app.models.core
{
    public enum UserRole
    {
        Student,
        Teacher,
        Admin
    }

    public enum UserStatus
    {
        Active,
        Inactive,
        SoftDelete,
    }

    public class User
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public string? Password { get; set; }
        public required UserRole Usertype { get; set; }
        public required UserStatus Status { get; set; }
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Phone { get; set; }
        public string? MicrosoftId { get; set; }
        public string? GoogleId{ get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
