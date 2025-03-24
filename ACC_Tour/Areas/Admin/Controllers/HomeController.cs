using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using ACC_Tour.Models;

namespace ACC_Tour.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var tours = await _context.Tours.ToListAsync();

            ViewBag.TotalTours = tours.Count;
            ViewBag.ActiveTours = tours.Count(t => t.IsActive);
            ViewBag.UpcomingTours = tours.Count(t => t.IsActive && t.StartDate > now);
            ViewBag.CompletedTours = tours.Count(t => t.IsActive && t.EndDate < now);

            // Lấy danh sách tour sắp tới (5 tour gần nhất)
            ViewBag.UpcomingToursList = tours
                .Where(t => t.IsActive && t.StartDate > now)
                .OrderBy(t => t.StartDate)
                .Take(5)
                .ToList();

            return View();
        }
    }
} 