using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using ACC_Tour.Models;
using ACC_Tour.Data;
using ACC_Tour.ViewModels;

namespace ACC_Tour.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TourController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TourController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Tour
        public async Task<IActionResult> Index()
        {
            var tours = await _context.Tours.ToListAsync();
            return View(tours);
        }

        // GET: Admin/Tour/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours
                .Include(t => t.Bookings)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }

        // GET: Admin/Tour/Create
        public IActionResult Create()
        {
            return View(new TourViewModel());
        }

        // POST: Admin/Tour/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TourViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var tour = new Tour
                {
                    Name = viewModel.Name,
                    Price = viewModel.Price,
                    StartDate = viewModel.StartDate,
                    EndDate = viewModel.EndDate,
                    MaxParticipants = viewModel.MaxParticipants,
                    MinParticipants = viewModel.MinParticipants,
                    Description = viewModel.Description,
                    IsActive = true
                };

                if (viewModel.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "tours");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.ImageFile.CopyToAsync(fileStream);
                    }

                    tour.ImageUrl = "/images/tours/" + uniqueFileName;
                }

                _context.Add(tour);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tour đã được thêm thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Admin/Tour/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours.FindAsync(id);
            if (tour == null)
            {
                return NotFound();
            }

            var viewModel = new TourViewModel
            {
                Id = tour.Id,
                Name = tour.Name,
                Price = tour.Price,
                StartDate = tour.StartDate,
                EndDate = tour.EndDate,
                MaxParticipants = tour.MaxParticipants,
                MinParticipants = tour.MinParticipants,
                Description = tour.Description,
                ImageUrl = tour.ImageUrl,
                IsActive = tour.IsActive
            };

            return View(viewModel);
        }

        // POST: Admin/Tour/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TourViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var tour = await _context.Tours.FindAsync(id);
                    if (tour == null)
                    {
                        return NotFound();
                    }

                    tour.Name = viewModel.Name;
                    tour.Price = viewModel.Price;
                    tour.StartDate = viewModel.StartDate;
                    tour.EndDate = viewModel.EndDate;
                    tour.MaxParticipants = viewModel.MaxParticipants;
                    tour.MinParticipants = viewModel.MinParticipants;
                    tour.Description = viewModel.Description;
                    tour.IsActive = viewModel.IsActive;

                    if (viewModel.ImageFile != null)
                    {
                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(tour.ImageUrl))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, tour.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Save new image
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "tours");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.ImageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await viewModel.ImageFile.CopyToAsync(fileStream);
                        }

                        tour.ImageUrl = "/images/tours/" + uniqueFileName;
                    }

                    _context.Update(tour);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Tour đã được cập nhật thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TourExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(viewModel);
        }

        // GET: Admin/Tour/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }

        // POST: Admin/Tour/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour != null)
            {
                tour.IsActive = false;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tour đã được xóa thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TourExists(int id)
        {
            return _context.Tours.Any(e => e.Id == id);
        }
    }
} 