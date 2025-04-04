using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ACC_Tour.Models;
using ACC_Tour.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

namespace ACC_Tour.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TourGuideController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TourGuideController> _logger;

        public TourGuideController(ApplicationDbContext context, ILogger<TourGuideController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Admin/TourGuide
        public async Task<IActionResult> Index()
        {
            try
            {
                var tourGuides = await _context.TourGuides.ToListAsync();
                return View(tourGuides);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving tour guides: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách hướng dẫn viên. Vui lòng thử lại sau.";
                return View(new List<TourGuide>());
            }
        }

        // GET: Admin/TourGuide/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hướng dẫn viên.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var tourGuide = await _context.TourGuides
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (tourGuide == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy hướng dẫn viên.";
                    return RedirectToAction(nameof(Index));
                }

                return View(tourGuide);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving tour guide details: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin hướng dẫn viên. Vui lòng thử lại sau.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Admin/TourGuide/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/TourGuide/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,PhoneNumber,Email,Experience,Languages,Description")] TourGuide tourGuide)
        {
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
                    return View(tourGuide);
                }

                // Kiểm tra email đã tồn tại chưa
                var existingEmail = await _context.TourGuides.AnyAsync(t => t.Email == tourGuide.Email);
                if (existingEmail)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng");
                    return View(tourGuide);
                }

                // Kiểm tra số điện thoại đã tồn tại chưa
                var existingPhone = await _context.TourGuides.AnyAsync(t => t.PhoneNumber == tourGuide.PhoneNumber);
                if (existingPhone)
                {
                    ModelState.AddModelError("PhoneNumber", "Số điện thoại này đã được sử dụng");
                    return View(tourGuide);
                }

                // Khởi tạo collection TourAssignments
                tourGuide.TourAssignments = new List<TourAssignment>();
                
                _context.Add(tourGuide);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Created new tour guide: {tourGuide.Name}");
                TempData["SuccessMessage"] = "Thêm hướng dẫn viên mới thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating tour guide: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm hướng dẫn viên. Vui lòng thử lại sau.";
                return View(tourGuide);
            }
        }

        // GET: Admin/TourGuide/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hướng dẫn viên.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var tourGuide = await _context.TourGuides.FindAsync(id);
                if (tourGuide == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy hướng dẫn viên.";
                    return RedirectToAction(nameof(Index));
                }
                return View(tourGuide);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving tour guide for edit: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin hướng dẫn viên. Vui lòng thử lại sau.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/TourGuide/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,PhoneNumber,Email,Experience,Languages,Description")] TourGuide tourGuide)
        {
            if (id != tourGuide.Id)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hướng dẫn viên.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            _logger.LogError($"Validation error: {error.ErrorMessage}");
                        }
                    }
                    TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin nhập vào";
                    return View(tourGuide);
                }

                // Kiểm tra email đã tồn tại chưa (trừ hướng dẫn viên hiện tại)
                var existingEmail = await _context.TourGuides.AnyAsync(t => t.Email == tourGuide.Email && t.Id != tourGuide.Id);
                if (existingEmail)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng");
                    TempData["ErrorMessage"] = "Email này đã được sử dụng";
                    return View(tourGuide);
                }

                // Kiểm tra số điện thoại đã tồn tại chưa (trừ hướng dẫn viên hiện tại)
                var existingPhone = await _context.TourGuides.AnyAsync(t => t.PhoneNumber == tourGuide.PhoneNumber && t.Id != tourGuide.Id);
                if (existingPhone)
                {
                    ModelState.AddModelError("PhoneNumber", "Số điện thoại này đã được sử dụng");
                    TempData["ErrorMessage"] = "Số điện thoại này đã được sử dụng";
                    return View(tourGuide);
                }

                _context.Update(tourGuide);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Updated tour guide: {tourGuide.Name}");
                TempData["SuccessMessage"] = "Cập nhật thông tin hướng dẫn viên thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TourGuideExists(tourGuide.Id))
                {
                    TempData["ErrorMessage"] = "Không tìm thấy hướng dẫn viên.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogError($"Concurrency error updating tour guide: {tourGuide.Id}");
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật thông tin. Vui lòng thử lại sau.";
                    return View(tourGuide);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating tour guide: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật thông tin. Vui lòng thử lại sau.";
                return View(tourGuide);
            }
        }

        // GET: Admin/TourGuide/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hướng dẫn viên.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var tourGuide = await _context.TourGuides
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (tourGuide == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy hướng dẫn viên.";
                    return RedirectToAction(nameof(Index));
                }

                return View(tourGuide);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving tour guide for delete: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin hướng dẫn viên. Vui lòng thử lại sau.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/TourGuide/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var tourGuide = await _context.TourGuides
                    .Include(tg => tg.TourAssignments)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (tourGuide == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy hướng dẫn viên.";
                    return RedirectToAction(nameof(Index));
                }

                // Kiểm tra xem hướng dẫn viên có tour assignment nào không
                var activeAssignments = tourGuide.TourAssignments
                    .Where(a => a.Status != AssignmentStatus.Cancelled)
                    .ToList();

                if (activeAssignments.Any())
                {
                    var assignmentDetails = string.Join(", ", activeAssignments
                        .Select(a => $"Tour {a.TourId} ({a.StartDate:dd/MM/yyyy} - {a.EndDate:dd/MM/yyyy})"));
                    
                    TempData["ErrorMessage"] = $"Không thể xóa hướng dẫn viên vì đã có tour được phân công: {assignmentDetails}. Vui lòng xóa các tour assignment trước.";
                    return RedirectToAction(nameof(Index));
                }

                // Xóa các tour assignment đã hủy
                if (tourGuide.TourAssignments.Any())
                {
                    _context.TourAssignments.RemoveRange(tourGuide.TourAssignments);
                }

                _context.TourGuides.Remove(tourGuide);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Deleted tour guide: {tourGuide.Name}");
                TempData["SuccessMessage"] = "Xóa hướng dẫn viên thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Database error deleting tour guide: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa hướng dẫn viên. Vui lòng kiểm tra lại các tour assignment và thử lại sau.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting tour guide: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa hướng dẫn viên. Vui lòng thử lại sau.";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool TourGuideExists(int id)
        {
            return _context.TourGuides.Any(e => e.Id == id);
        }
    }
} 