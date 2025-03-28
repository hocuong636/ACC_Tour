using ACC_Tour.Data;
using ACC_Tour.Models;
using ACC_Tour.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
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

        public DescriptionImageController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: DescriptionImage
        public async Task<IActionResult> Index()
        {
            var descriptionImages = await _context.DescriptionImages
                .Include(d => d.Tour)
                .ToListAsync();
            return View(descriptionImages);
        }

        // GET: DescriptionImage/Create
        public IActionResult Create()
        {
            ViewData["TourId"] = new SelectList(_context.Tours, "Id", "Name");
            return View();
        }

        // POST: DescriptionImage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DescriptionImageViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "tours");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var descriptionImages = new List<DescriptionImage>();

                    foreach (var imageFile in model.ImageFiles)
                    {
                        if (imageFile.Length > 0)
                        {
                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(fileStream);
                            }

                            descriptionImages.Add(new DescriptionImage
                            {
                                TourId = model.TourId,
                                ImageFile = "/images/tours/" + uniqueFileName
                            });
                        }
                    }

                    if (descriptionImages.Any())
                    {
                        await _context.DescriptionImages.AddRangeAsync(descriptionImages);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu ảnh: " + ex.Message);
                }
            }

            ViewData["TourId"] = new SelectList(_context.Tours, "Id", "Name", model.TourId);
            return View(model);
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
            if (descriptionImage != null)
            {
                // Xóa file ảnh
                if (!string.IsNullOrEmpty(descriptionImage.ImageFile))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, descriptionImage.ImageFile.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.DescriptionImages.Remove(descriptionImage);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DescriptionImageExists(int id)
        {
            return _context.DescriptionImages.Any(e => e.Id == id);
        }
    }
}
