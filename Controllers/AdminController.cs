using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CineScore.Models;
using CineScore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CineScore.Controllers;

public class AdminController : Controller
{
    private readonly CineScoreContext _context;
    public AdminController(CineScoreContext context)
    {
        _context = context;
    }

    // GET: Admin/Index
    [Authorize(Roles = "Admin")]
    public IActionResult Index()
    {
        return View();
    }

    // GET: Admin/Users
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Users()
    {
        var users = await _context.Users.ToListAsync();
        return View(users);
    }
}
