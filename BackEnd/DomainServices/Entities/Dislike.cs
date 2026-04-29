namespace Backend.Domain.Entities
{
    public class Dislike
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string DislikedUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
