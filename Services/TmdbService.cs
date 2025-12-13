using System.Net.Http.Headers;
using System.Net.Http.Json;
using CineScore.Data;
using CineScore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

namespace CineScore.Services
{
    public class TmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly CineScoreContext _context;
        private readonly TmdbOptions _options;
        private IReadOnlyDictionary<int, string>? _genreLookup;

        public TmdbService(HttpClient httpClient, IOptions<TmdbOptions> options, CineScoreContext context)
        {
            _httpClient = httpClient;
            _context = context;
            _options = options.Value;

            _httpClient.BaseAddress = new Uri("https://api.themoviedb.org/3/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ReadAccessToken);
        }

        public async Task<PagedMoviesResult> GetPopularMoviesAsync(int page)
        {
            var response = await _httpClient.GetFromJsonAsync<TmdbResponse>($"movie/popular?language=en-US&page={page}&api_key={_options.ApiKey}");
            return await MapAndPersistAsync(response, page);
        }

        public async Task<PagedMoviesResult> GetTopRatedMoviesAsync(int page)
        {
            var response = await _httpClient.GetFromJsonAsync<TmdbResponse>($"movie/top_rated?language=en-US&page={page}&api_key={_options.ApiKey}");
            return await MapAndPersistAsync(response, page);
        }

        private async Task<PagedMoviesResult> MapAndPersistAsync(TmdbResponse? response, int page)
        {
            if (response?.Results == null)
            {
                return new PagedMoviesResult(Enumerable.Empty<Movie>(), page, _options.MaxPages);
            }

            await EnsureGenresAsync();

            var mappedMovies = response.Results
                .Select(MapMovie)
                .Take(_options.PageSize)
                .ToList();

            var persistedMovies = await UpsertMoviesAsync(mappedMovies);

            var totalPages = Math.Max(1, Math.Min(response.TotalPages, _options.MaxPages));

            return new PagedMoviesResult(persistedMovies, Math.Clamp(page, 1, totalPages), totalPages);
        }

        private async Task EnsureGenresAsync()
        {
            if (_genreLookup != null)
            {
                return;
            }

            var genreResponse = await _httpClient.GetFromJsonAsync<TmdbGenreResponse>($"genre/movie/list?language=en&api_key={_options.ApiKey}");
            _genreLookup = genreResponse?.Genres?.ToDictionary(g => g.Id, g => g.Name) ?? new Dictionary<int, string>();
        }

        private Movie MapMovie(TmdbMovie source)
        {
            var releaseYear = 0;
            if (DateTime.TryParse(source.ReleaseDate, out var releaseDate))
            {
                releaseYear = releaseDate.Year;
            }

            var genres = source.GenreIds
                .Where(id => _genreLookup != null && _genreLookup.ContainsKey(id))
                .Select(id => _genreLookup![id])
                .ToList();

            var hasBackdrop = !string.IsNullOrWhiteSpace(source.BackdropPath);
            var hasPoster = !string.IsNullOrWhiteSpace(source.PosterPath);
            var bannerUrl = hasBackdrop
                ? $"{_options.BannerImageBaseUrl}{source.BackdropPath}"
                : hasPoster
                    ? $"{_options.BannerFallbackPosterBaseUrl}{source.PosterPath}"
                    : null;

            return new Movie
            {
                Title = string.IsNullOrWhiteSpace(source.Title) ? source.OriginalTitle ?? "Untitled" : source.Title,
                Description = string.IsNullOrWhiteSpace(source.Overview) ? "No description provided." : source.Overview,
                Genre = genres.Any() ? string.Join(", ", genres) : "Uncategorized",
                Year = releaseYear,
                PosterUrl = hasPoster ? $"{_options.ImageBaseUrl}{source.PosterPath}" : null,
                BannerUrl = bannerUrl
            };
        }

        private async Task<IEnumerable<Movie>> UpsertMoviesAsync(IEnumerable<Movie> movies)
        {
            var results = new List<Movie>();

            await SchemaGuard.EnsureBannerColumnAsync(_context);

            foreach (var movie in movies)
            {
                var existingMovie = await _context.Movies.FirstOrDefaultAsync(m => m.Title == movie.Title && m.Year == movie.Year);

                if (existingMovie == null)
                {
                    _context.Movies.Add(movie);
                    results.Add(movie);
                }
                else
                {
                    existingMovie.Year = movie.Year;
                    existingMovie.Genre = movie.Genre;
                    existingMovie.Description = movie.Description;
                    existingMovie.PosterUrl = movie.PosterUrl;
                    existingMovie.BannerUrl = movie.BannerUrl;
                    results.Add(existingMovie);
                }
            }

            await _context.SaveChangesAsync();

            return results;
        }

        private class TmdbResponse
        {
            [JsonPropertyName("page")]
            public int Page { get; set; }

            [JsonPropertyName("total_pages")]
            public int TotalPages { get; set; }

            [JsonPropertyName("results")]
            public List<TmdbMovie>? Results { get; set; }
        }

        private class TmdbMovie
        {
            [JsonPropertyName("title")]
            public string? Title { get; set; }

            [JsonPropertyName("original_title")]
            public string? OriginalTitle { get; set; }

            [JsonPropertyName("overview")]
            public string? Overview { get; set; }

            [JsonPropertyName("release_date")]
            public string? ReleaseDate { get; set; }

            [JsonPropertyName("poster_path")]
            public string? PosterPath { get; set; }

            [JsonPropertyName("backdrop_path")]
            public string? BackdropPath { get; set; }

            [JsonPropertyName("genre_ids")]
            public List<int> GenreIds { get; set; } = new();
        }

        private class TmdbGenreResponse
        {
            [JsonPropertyName("genres")]
            public List<TmdbGenre>? Genres { get; set; }
        }

        private class TmdbGenre
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;
        }
    }
}
