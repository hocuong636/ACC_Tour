using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ACC_Tour.Data;
using ACC_Tour.Models;

namespace ACC_Tour.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Review
        public async Task<IActionResult> Index(int? page, string searchString, int? tourFilter, int? ratingFilter, bool? approvedFilter)
        {
            var reviews = _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Tour)
                .AsQueryable();

            // Tìm kiếm theo tên người dùng hoặc bình luận
            if (!string.IsNullOrEmpty(searchString))
            {
                reviews = reviews.Where(r => 
                    r.User.FullName.Contains(searchString) || 
                    r.Comment.Contains(searchString) ||
                    r.Tour.Name.Contains(searchString));
            }

            // Lọc theo tour
            if (tourFilter.HasValue)
            {
                reviews = reviews.Where(r => r.TourId == tourFilter.Value);
            }

            // Lọc theo số sao
            if (ratingFilter.HasValue)
            {
                reviews = reviews.Where(r => r.Rating == ratingFilter.Value);
            }

            // Lọc theo trạng thái duyệt
            if (approvedFilter.HasValue)
            {
                reviews = reviews.Where(r => r.IsApproved == approvedFilter.Value);
            }

            // Sắp xếp theo ngày mới nhất trước
            reviews = reviews.OrderByDescending(r => r.ReviewDate);

            // Lấy danh sách tour để filter
            ViewBag.Tours = await _context.Tours.ToListAsync();

            return View(await reviews.ToListAsync());
        }

        // GET: Admin/Review/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Tour)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Admin/Review/Approve/5
        public async Task<IActionResult> Approve(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Tour)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            if (review.IsApproved)
            {
                TempData["InfoMessage"] = "Đánh giá này đã được duyệt rồi.";
                return RedirectToAction(nameof(Index));
            }

            review.IsApproved = true;
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đánh giá đã được duyệt thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Review/Reject/5
        public async Task<IActionResult> Reject(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Tour)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Admin/Review/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            review.IsApproved = false;
            review.AdminResponse = $"Từ chối: {reason}";
            
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đánh giá đã bị từ chối.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Review/AddResponse/5
        public async Task<IActionResult> AddResponse(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Tour)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Admin/Review/AddResponse/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddResponse(int id, string adminResponse)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(adminResponse))
            {
                ModelState.AddModelError("adminResponse", "Vui lòng nhập phản hồi.");
                return View(review);
            }

            review.AdminResponse = adminResponse;
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Phản hồi đã được thêm thành công!";
            return RedirectToAction(nameof(Details), new { id = review.Id });
        }

        // POST: Admin/Review/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đánh giá." });
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đánh giá đã bị xóa." });
        }

        // GET: Admin/Review/Statistics
        public async Task<IActionResult> Statistics()
        {
            var totalReviews = await _context.Reviews.CountAsync();
            var approvedReviews = await _context.Reviews.Where(r => r.IsApproved).CountAsync();
            var pendingReviews = await _context.Reviews.Where(r => !r.IsApproved).CountAsync();

            var ratingDistribution = await _context.Reviews
                .GroupBy(r => r.Rating)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .OrderBy(x => x.Rating)
                .ToListAsync();

            var averageRating = await _context.Reviews
                .Where(r => r.IsApproved)
                .AverageAsync(r => (double?)r.Rating) ?? 0;

            ViewBag.TotalReviews = totalReviews;
            ViewBag.ApprovedReviews = approvedReviews;
            ViewBag.PendingReviews = pendingReviews;
            ViewBag.AverageRating = Math.Round(averageRating, 2);
            ViewBag.RatingDistribution = ratingDistribution;

            return View();
        }
    }
}
