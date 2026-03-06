namespace backend.app.models.core
{
    public class Contact
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string Topic { get; set; }
        public required string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
