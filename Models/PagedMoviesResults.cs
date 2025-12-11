namespace CineScore.Models
{
    public class PagedMoviesResult
    {
        public PagedMoviesResult(IEnumerable<Movie> movies, int currentPage, int totalPages)
        {
            Movies = movies;
            CurrentPage = currentPage;
            TotalPages = totalPages;
        }

        public IEnumerable<Movie> Movies { get; }
        public int CurrentPage { get; }
        public int TotalPages { get; }
    }
}