namespace CineScore.Models
{
    public class TmdbOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ReadAccessToken { get; set; } = string.Empty;
        public string ImageBaseUrl { get; set; } = "https://image.tmdb.org/t/p/w500";
        public int PageSize { get; set; } = 12;
        public int MaxPages { get; set; } = 5;
    }
}