using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CineScore.Models;
using CineScore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CineScore.Controllers;

public class HomeController : Controller
{
    private readonly CineScoreContext _context;
    public HomeController(CineScoreContext context)
    {
        _context = context;
    }

    // GET: Home/Admin_dashboard
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Admin_dashboard()
    {
        return View();
    }

    public async Task<IActionResult> Index()
    {
        var movies = await _context.Movies.AsNoTracking().ToListAsync();
        return View(movies);
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
