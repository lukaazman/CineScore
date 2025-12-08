using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CineScore.Models;
using CineScore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CineScore.Controllers;

public class UserController : Controller
{
    private readonly CineScoreContext _context;
    public UserController(CineScoreContext context)
    {
        _context = context;
    }

    // GET: User/Index
    [AllowAnonymous]
    public async Task<IActionResult> Index(string? id)
    {
        string userId = id ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var user = await _context.Users
            .Include(u => u.Comments)
            .Include(u => u.Favorites)
                .ThenInclude(f => f.Movie)
            .FirstOrDefaultAsync(u => u.Id == userId);

        var comments = await _context.Comments
            .Include(c => c.Movie)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        ViewBag.Comments = comments;
        return View(user);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddFavorite(int movieId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        // Check if already favorited
        bool alreadyFavorited = await _context.Favorites
            .AnyAsync(f => f.UserId == userId && f.MovieId == movieId);

        if (!alreadyFavorited)
        {
            var favorite = new Favorite
            {
                UserId = userId,
                MovieId = movieId
            };
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
        }

        // Redirect back to the movie details page
        return RedirectToAction("Details_user", "Movies", new { id = movieId });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RemoveFavorite(int movieId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        var favorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.MovieId == movieId);

        if (favorite != null)
        {
            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Details_user", "Movies", new { id = movieId });
    }
}