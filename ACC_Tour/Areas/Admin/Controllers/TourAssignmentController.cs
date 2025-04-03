using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ACC_Tour.Models;
using ACC_Tour.Data;

namespace ACC_Tour.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TourAssignmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TourAssignmentController> _logger;

        public TourAssignmentController(ApplicationDbContext context, ILogger<TourAssignmentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Admin/TourAssignment
        public async Task<IActionResult> Index()
        {
            try
            {
                var assignments = await _context.TourAssignments
                    .Include(a => a.Tour)
                    .Include(a => a.TourGuide)
                    .OrderByDescending(a => a.StartDate)
                    .ToListAsync();
                return View(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving tour assignments: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách phân công. Vui lòng thử lại sau.";
                return View(new List<TourAssignment>());
            }
        }

        // GET: Admin/TourAssignment/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Tours = await _context.Tours.Where(t => t.IsActive).ToListAsync();
                ViewBag.TourGuides = await _context.TourGuides.ToListAsync();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading create form: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải form tạo mới. Vui lòng thử lại sau.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Admin/TourAssignment/GetTourInfo
        [HttpGet]
        public async Task<IActionResult> GetTourInfo(int tourId)
        {
            try
            {
                var tour = await _context.Tours.FindAsync(tourId);
                if (tour == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy tour" });
                }

                return Json(new { 
                    success = true, 
                    startDate = tour.StartDate.ToString("yyyy-MM-dd"),
                    endDate = tour.EndDate.ToString("yyyy-MM-dd")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting tour info: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi lấy thông tin tour" });
            }
        }

        // GET: Admin/TourAssignment/GetAvailableTourGuides
        [HttpGet]
        public async Task<IActionResult> GetAvailableTourGuides(int tourId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var tour = await _context.Tours.FindAsync(tourId);
                if (tour == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy tour" });
                }

                // Lấy danh sách hướng dẫn viên đã có lịch trong khoảng thời gian này
                var busyGuides = await _context.TourAssignments
                    .Where(a => a.Status != AssignmentStatus.Cancelled &&
                               ((a.StartDate <= startDate && a.EndDate >= startDate) ||
                                (a.StartDate <= endDate && a.EndDate >= endDate) ||
                                (a.StartDate >= startDate && a.EndDate <= endDate)))
                    .Select(a => a.TourGuideId)
                    .ToListAsync();

                // Lấy danh sách hướng dẫn viên khả dụng
                var availableGuides = await _context.TourGuides
                    .Where(g => !busyGuides.Contains(g.Id))
                    .Select(g => new { id = g.Id, name = g.Name })
                    .ToListAsync();

                return Json(new { success = true, guides = availableGuides });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting available tour guides: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi lấy danh sách hướng dẫn viên" });
            }
        }

        // POST: Admin/TourAssignment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TourId,TourGuideId,StartDate,EndDate,Notes")] TourAssignment assignment)
        {
            try
            {
                // Lấy thông tin tour để kiểm tra ngày
                var tour = await _context.Tours.FindAsync(assignment.TourId);
                if (tour == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy tour");
                    ViewBag.Tours = await _context.Tours.Where(t => t.IsActive).ToListAsync();
                    ViewBag.TourGuides = await _context.TourGuides.ToListAsync();
                    return View(assignment);
                }

                // Gán ngày từ tour vào assignment
                assignment.StartDate = tour.StartDate;
                assignment.EndDate = tour.EndDate;

                // Kiểm tra xem hướng dẫn viên có lịch trùng không
                var hasConflict = await _context.TourAssignments
                    .AnyAsync(a => a.TourGuideId == assignment.TourGuideId &&
                                 a.Status != AssignmentStatus.Cancelled &&
                                 ((a.StartDate <= assignment.StartDate && a.EndDate >= assignment.StartDate) ||
                                  (a.StartDate <= assignment.EndDate && a.EndDate >= assignment.EndDate) ||
                                  (a.StartDate >= assignment.StartDate && a.EndDate <= assignment.EndDate)));

                if (hasConflict)
                {
                    ModelState.AddModelError("", "Hướng dẫn viên đã có lịch trong khoảng thời gian này");
                    ViewBag.Tours = await _context.Tours.Where(t => t.IsActive).ToListAsync();
                    ViewBag.TourGuides = await _context.TourGuides.ToListAsync();
                    return View(assignment);
                }

                if (ModelState.IsValid)
                {
                    assignment.Status = AssignmentStatus.Pending;
                    _context.Add(assignment);
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation($"Created new tour assignment: Tour {assignment.TourId}, Guide {assignment.TourGuideId}");
                    TempData["SuccessMessage"] = "Thêm phân công tour mới thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                .Select(e => e.ErrorMessage);
                    foreach (var error in errors)
                    {
                        _logger.LogError($"Validation error: {error}");
                    }
                    ViewBag.Tours = await _context.Tours.Where(t => t.IsActive).ToListAsync();
                    ViewBag.TourGuides = await _context.TourGuides.ToListAsync();
                    return View(assignment);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating tour assignment: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm phân công tour. Vui lòng thử lại sau.";
                ViewBag.Tours = await _context.Tours.Where(t => t.IsActive).ToListAsync();
                ViewBag.TourGuides = await _context.TourGuides.ToListAsync();
                return View(assignment);
            }
        }

        // GET: Admin/TourAssignment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phân công tour.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var assignment = await _context.TourAssignments.FindAsync(id);
                if (assignment == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy phân công tour.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Tours = await _context.Tours.Where(t => t.IsActive).ToListAsync();
                ViewBag.TourGuides = await _context.TourGuides.ToListAsync();
                return View(assignment);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving tour assignment for edit: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin phân công tour. Vui lòng thử lại sau.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/TourAssignment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TourId,TourGuideId,StartDate,EndDate,Status,Notes")] TourAssignment assignment)
        {
            if (id != assignment.Id)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phân công tour.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                .Select(e => e.ErrorMessage);
                    foreach (var error in errors)
                    {
                        _logger.LogError($"Validation error: {error}");
                    }
                    ViewBag.Tours = await _context.Tours.Where(t => t.IsActive).ToListAsync();
                    ViewBag.TourGuides = await _context.TourGuides.ToListAsync();
                    return View(assignment);
                }

                // Kiểm tra xem hướng dẫn viên có lịch trùng không (trừ assignment hiện tại)
                var hasConflict = await _context.TourAssignments
                    .AnyAsync(a => a.Id != assignment.Id &&
                                 a.TourGuideId == assignment.TourGuideId &&
                                 a.Status != AssignmentStatus.Cancelled &&
                                 ((a.StartDate <= assignment.StartDate && a.EndDate >= assignment.StartDate) ||
                                  (a.StartDate <= assignment.EndDate && a.EndDate >= assignment.EndDate) ||
                                  (a.StartDate >= assignment.StartDate && a.EndDate <= assignment.EndDate)));

                if (hasConflict)
                {
                    ModelState.AddModelError("", "Hướng dẫn viên đã có lịch trong khoảng thời gian này");
                    ViewBag.Tours = await _context.Tours.Where(t => t.IsActive).ToListAsync();
                    ViewBag.TourGuides = await _context.TourGuides.ToListAsync();
                    return View(assignment);
                }

                // Kiểm tra xem tour có được phân công cho hướng dẫn viên khác trong khoảng thời gian này không (trừ assignment hiện tại)
                var tourConflict = await _context.TourAssignments
                    .AnyAsync(a => a.Id != assignment.Id &&
                                 a.TourId == assignment.TourId &&
                                 a.Status != AssignmentStatus.Cancelled &&
                                 ((a.StartDate <= assignment.StartDate && a.EndDate >= assignment.StartDate) ||
                                  (a.StartDate <= assignment.EndDate && a.EndDate >= assignment.EndDate) ||
                                  (a.StartDate >= assignment.StartDate && a.EndDate <= assignment.EndDate)));

                if (tourConflict)
                {
                    ModelState.AddModelError("", "Tour này đã được phân công cho hướng dẫn viên khác trong khoảng thời gian này");
                    ViewBag.Tours = await _context.Tours.Where(t => t.IsActive).ToListAsync();
                    ViewBag.TourGuides = await _context.TourGuides.ToListAsync();
                    return View(assignment);
                }

                _context.Update(assignment);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Updated tour assignment: {assignment.Id}");
                TempData["SuccessMessage"] = "Cập nhật phân công tour thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TourAssignmentExists(assignment.Id))
                {
                    TempData["ErrorMessage"] = "Không tìm thấy phân công tour.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogError($"Concurrency error updating tour assignment: {assignment.Id}");
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật thông tin. Vui lòng thử lại sau.";
                    ViewBag.Tours = await _context.Tours.Where(t => t.IsActive).ToListAsync();
                    ViewBag.TourGuides = await _context.TourGuides.ToListAsync();
                    return View(assignment);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating tour assignment: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật thông tin. Vui lòng thử lại sau.";
                ViewBag.Tours = await _context.Tours.Where(t => t.IsActive).ToListAsync();
                ViewBag.TourGuides = await _context.TourGuides.ToListAsync();
                return View(assignment);
            }
        }

        // GET: Admin/TourAssignment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phân công tour.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var assignment = await _context.TourAssignments
                    .Include(a => a.Tour)
                    .Include(a => a.TourGuide)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (assignment == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy phân công tour.";
                    return RedirectToAction(nameof(Index));
                }

                return View(assignment);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving tour assignment for delete: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin phân công tour. Vui lòng thử lại sau.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/TourAssignment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var assignment = await _context.TourAssignments.FindAsync(id);
                if (assignment == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy phân công tour.";
                    return RedirectToAction(nameof(Index));
                }

                _context.TourAssignments.Remove(assignment);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Deleted tour assignment: {id}");
                TempData["SuccessMessage"] = "Xóa phân công tour thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting tour assignment: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa phân công tour. Vui lòng thử lại sau.";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool TourAssignmentExists(int id)
        {
            return _context.TourAssignments.Any(e => e.Id == id);
        }
    }
} 