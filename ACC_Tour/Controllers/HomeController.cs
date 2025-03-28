using System.Diagnostics;
using ACC_Tour.Data;
using ACC_Tour.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ACC_Tour.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var tours = await _context.Tours
                .Where(t => t.IsActive)
                .OrderByDescending(t => t.StartDate)
                .Take(6)
                .ToListAsync();
            return View(tours);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Services()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Destination()
        {
            return View();
        }
        public IActionResult Gallery()
        {
            return View();
        }
        public IActionResult Testimonial()
        {
            return View();
        }
        public async Task<IActionResult> Blog()
        {
            var blogs = await _context.Blogs
                .Where(b => b.IsPublished)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
            return View(blogs);
        }

        public async Task<IActionResult> BlogDetail(string slug)
        {
            var blog = await _context.Blogs
                .FirstOrDefaultAsync(b => b.Slug == slug && b.IsPublished);

            if (blog == null)
            {
                return NotFound();
            }

            var recentBlogs = await _context.Blogs
                .Where(b => b.IsPublished && b.Id != blog.Id)
                .OrderByDescending(b => b.CreatedAt)
                .Take(3)
                .ToListAsync();

            ViewBag.RecentBlogs = recentBlogs;
            return View(blog);
        }

        public IActionResult Booking()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
