public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int UserId { get; set; }
    public int MovieId { get; set; }

    public User? User { get; set; }
    public Movie? Movie { get; set; }
}
