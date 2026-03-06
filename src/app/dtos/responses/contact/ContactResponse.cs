namespace backend.app.dtos.responses.contact
{
    public class ContactResponse
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string Topic { get; set; }
        public required string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
