using ACC_Tour.Data;
using ACC_Tour.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ACC_Tour.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class DescriptionImageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DescriptionImageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DescriptionImage
        public async Task<IActionResult> Index()
        {
            var descriptionImages = _context.DescriptionImages.Include(d => d.Tour);
            return View(await descriptionImages.ToListAsync());
        }

        // GET: DescriptionImage/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var descriptionImage = await _context.DescriptionImages
                .Include(d => d.Tour)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (descriptionImage == null) return NotFound();

            return View(descriptionImage);
        }

        // GET: DescriptionImage/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DescriptionImage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DescriptionImage model)
        {
            if (ModelState.IsValid)
            {
                var descriptionImage = new DescriptionImage
                {
                    TourId = model.TourId
                };
                
                if (model.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "tours");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        //await model.ImageFile(fileStream);
                    }

                    descriptionImage.ImageFile = "/images/tours/" + uniqueFileName;
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: DescriptionImage/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var descriptionImage = await _context.DescriptionImages.FindAsync(id);
            if (descriptionImage == null) return NotFound();

            return View(descriptionImage);
        }

        // POST: DescriptionImage/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Url,TourId")] DescriptionImage descriptionImage)
        {
            if (id != descriptionImage.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(descriptionImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DescriptionImageExists(descriptionImage.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(descriptionImage);
        }

        // GET: DescriptionImage/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var descriptionImage = await _context.DescriptionImages
                .Include(d => d.Tour)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (descriptionImage == null) return NotFound();

            return View(descriptionImage);
        }

        // POST: DescriptionImage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var descriptionImage = await _context.DescriptionImages.FindAsync(id);
            _context.DescriptionImages.Remove(descriptionImage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DescriptionImageExists(int id)
        {
            return _context.DescriptionImages.Any(e => e.Id == id);
        }
    }
}
