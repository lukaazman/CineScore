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
            .FirstOrDefaultAsync(u => u.Id == userId);

        var comments = await _context.Comments
            .Include(c => c.Movie)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        ViewBag.Comments = comments;
        return View(user);
    }
}