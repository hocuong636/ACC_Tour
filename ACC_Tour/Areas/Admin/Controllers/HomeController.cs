using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ACC_Tour.Data;

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
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.Description,
                    t.StartDate,
                    t.EndDate,
                    t.Price,
                    t.ImageUrl,
                    Status = "Sắp tới"
                })
                .ToList();

            return View();
        }
    }
} 