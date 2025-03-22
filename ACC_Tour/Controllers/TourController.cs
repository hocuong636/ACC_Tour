using Microsoft.AspNetCore.Mvc;
using ACC_Tour.Models;
using Microsoft.EntityFrameworkCore;
using Data;

namespace ACC_Tour.Controllers
{
    public class TourController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TourController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var tours = from t in _context.Tours
                       select t;

            if (!String.IsNullOrEmpty(searchString))
            {
                tours = tours.Where(t => t.Name.Contains(searchString) 
                                    || t.Description.Contains(searchString));
            }

            return View(await tours.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours
                .Include(t => t.Reviews)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }
    }
}
