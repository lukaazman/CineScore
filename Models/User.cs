public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";

    public ICollection<Rating>? Ratings { get; set; }
    public ICollection<Comment>? Comments { get; set; }
    public ICollection<Favorite>? Favorites { get; set; }
    //public ICollection<Watchlist>? Watchlist { get; set; }
}
