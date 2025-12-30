using System.Diagnostics;
using CineScore.Data;
using CineScore.Models;
using CineScore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;

namespace CineScore.Controllers;

public class HomeController : Controller
{
    private readonly CineScoreContext _context;
    private readonly TmdbService _tmdbService;
    private readonly TmdbOptions _tmdbOptions;

    public HomeController(CineScoreContext context, TmdbService tmdbService, IOptions<TmdbOptions> tmdbOptions)
    {
        _context = context;
        _tmdbService = tmdbService;
        _tmdbOptions = tmdbOptions.Value;
    }

    // GET: Home/Admin_dashboard
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Admin_dashboard()
    {
        return View();
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        var clampedPage = Math.Clamp(page, 1, _tmdbOptions.MaxPages);
        var popular = await _tmdbService.GetPopularMoviesAsync(clampedPage);

        var movieIds = popular.Movies.Select(m => m.Id).ToList();
        var movies = await _context.Movies
            .Where(m => movieIds.Contains(m.Id))
            .AsNoTracking()
            .ToListAsync();

        ViewData["Title"] = "Home";
        ViewData["Heading"] = "Discover Movies";
        ViewData["Lead"] = "Browse the latest popular titles.";

        return View("Index", new PagedMoviesResult(movies, popular.CurrentPage, popular.TotalPages));
    }

    public async Task<IActionResult> Search(string? query, int page = 1)
    {
        var trimmedQuery = query?.Trim() ?? string.Empty;
        var clampedPage = Math.Clamp(page, 1, _tmdbOptions.MaxPages);
        var aggregatedMovies = new List<Movie>();
        PagedMoviesResult? latestResult = null;
        var maxPagesToFetch = _tmdbOptions.MaxPages;

        for (var p = 1; p <= maxPagesToFetch; p++)
        {
            var searchResults = await _tmdbService.SearchMoviesAsync(trimmedQuery, p);
            latestResult = searchResults;

            aggregatedMovies.AddRange(searchResults.Movies);

            if (p >= searchResults.TotalPages)
            {
                break;
            }
        }

        latestResult ??= new PagedMoviesResult(Enumerable.Empty<Movie>(), 1, 1, trimmedQuery);

        ViewData["Title"] = "Search";
        ViewData["Heading"] = string.IsNullOrWhiteSpace(trimmedQuery)
            ? "Search Movies"
            : $"Results for \"{trimmedQuery}\"";
        ViewData["Lead"] = string.IsNullOrWhiteSpace(trimmedQuery)
            ? "Find movies by name."
            : "Browse the closest matches to your search.";
        ViewData["SearchQuery"] = trimmedQuery;

        var filteredMovies = aggregatedMovies
            .Where(movie => !(string.IsNullOrWhiteSpace(movie.PosterUrl)
                && string.Equals(movie.Genre, "Uncategorized", StringComparison.OrdinalIgnoreCase)))
            .ToList();

        var pageSize = _tmdbOptions.PageSize;
        var filteredTotalPages = Math.Max(1, (int)Math.Ceiling(filteredMovies.Count / (double)pageSize));
        var boundedTotalPages = Math.Max(1, Math.Min(filteredTotalPages, maxPagesToFetch));
        var currentPage = Math.Clamp(clampedPage, 1, boundedTotalPages);

        var pagedMovies = filteredMovies
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var filteredResults = new PagedMoviesResult(
            pagedMovies,
            currentPage,
            boundedTotalPages,
            latestResult.SearchQuery);

        return View("Search", filteredResults);
    }

    public async Task<IActionResult> TopRated(int page = 1)
    {
        var clampedPage = Math.Clamp(page, 1, _tmdbOptions.MaxPages);
        var topRated = await _tmdbService.GetTopRatedMoviesAsync(clampedPage);

        var movieIds = topRated.Movies.Select(m => m.Id).ToList();
        var movies = await _context.Movies
            .Where(m => movieIds.Contains(m.Id))
            .Include(m => m.Comments)
            .AsNoTracking()
            .Select(m => new
            {
                Movie = m,
                AverageRating = m.Comments
                    .Where(c => c.Rating > 0)
                    .Average(c => (double?)c.Rating) ?? 0
            })
            .OrderByDescending(result => result.AverageRating)
            .ThenBy(result => result.Movie.Title)
            .Select(result => result.Movie)
            .ToListAsync();

        ViewData["Title"] = "Top Rated";
        ViewData["Heading"] = "Top Rated Movies";
        ViewData["Lead"] = "Movies sorted by community ratings.";

        return View("TopRated", new PagedMoviesResult(movies, topRated.CurrentPage, topRated.TotalPages));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
