namespace Backend.Domain.Entities
{
    public class Like
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string LikedUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Mutual { get; set; }
    }
}
