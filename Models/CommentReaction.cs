using System;

namespace CineScore.Models
{
    public class CommentReaction
    {
        public int Id { get; set; }
        public int CommentId { get; set; }
        public required string UserId { get; set; }
        public bool IsLike { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Comment? Comment { get; set; }
        public User? User { get; set; }
    }
}
