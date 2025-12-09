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
        ViewData["Lead"] = "Browse the latest popular titles from TMDB.";

        return View("Index", new PagedMoviesResult(movies, popular.CurrentPage, popular.TotalPages));
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
        ViewData["Lead"] = "Movies sorted by community ratings. Showing only the best.";

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
