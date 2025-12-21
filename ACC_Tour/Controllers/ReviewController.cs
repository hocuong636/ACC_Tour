using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ACC_Tour.Data;
using ACC_Tour.Models;
using System.Security.Claims;

namespace ACC_Tour.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Review/Create
        public async Task<IActionResult> Create(int tourId)
        {
            var tour = await _context.Tours.FindAsync(tourId);
            if (tour == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Kiểm tra xem user đã tham gia tour này chưa
            var hasParticipated = await _context.Bookings
                .Where(b => b.UserId == userId && b.TourId == tourId 
                    && (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed))
                .AnyAsync();

            if (!hasParticipated)
            {
                // Hiển thị trang chi tiết tour với thông báo lỗi
                ViewBag.ErrorMessage = "Bạn cần trải nghiệm tour trước khi đánh giá";
                ViewBag.ShowError = true;
                // Load tour với reviews
                var tourWithReviews = await _context.Tours
                    .Include(t => t.Reviews)
                        .ThenInclude(r => r.User)
                    .Include(t => t.DescriptionImages)
                    .FirstOrDefaultAsync(t => t.Id == tourId && t.IsActive == true);
                
                if (tourWithReviews == null)
                {
                    return NotFound();
                }

                return View("~/Views/Tour/Details.cshtml", tourWithReviews);
            }

            // Kiểm tra xem user đã viết review cho tour này chưa
            var existingReview = await _context.Reviews
                .Where(r => r.UserId == userId && r.TourId == tourId)
                .FirstOrDefaultAsync();

            if (existingReview != null)
            {
                ViewBag.InfoMessage = "Bạn đã viết đánh giá cho tour này rồi";
                ViewBag.ShowInfo = true;
                // Load tour với reviews
                var tourWithReviews = await _context.Tours
                    .Include(t => t.Reviews)
                        .ThenInclude(r => r.User)
                    .Include(t => t.DescriptionImages)
                    .FirstOrDefaultAsync(t => t.Id == tourId && t.IsActive == true);
                
                if (tourWithReviews == null)
                {
                    return NotFound();
                }

                return View("~/Views/Tour/Details.cshtml", tourWithReviews);
            }

            var review = new Review
            {
                TourId = tourId
            };

            return View(review);
        }

        // POST: Review/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int tourId, int rating, string comment)
        {
            // Validate dữ liệu từ form
            if (string.IsNullOrWhiteSpace(comment))
            {
                ModelState.AddModelError("Comment", "Vui lòng nhập nhận xét");
            }
            else if (comment.Length < 10)
            {
                ModelState.AddModelError("Comment", "Nhận xét phải ít nhất 10 ký tự");
            }
            else if (comment.Length > 1000)
            {
                ModelState.AddModelError("Comment", "Nhận xét không quá 1000 ký tự");
            }

            if (rating < 1 || rating > 5)
            {
                ModelState.AddModelError("Rating", "Vui lòng chọn số sao từ 1 đến 5");
            }

            if (!ModelState.IsValid)
            {
                // Return view chi tiết tour với form lỗi
                var review = new Review { TourId = tourId, Rating = rating, Comment = comment };
                return View(review);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Kiểm tra xem user đã tham gia tour này chưa
            var hasParticipated = await _context.Bookings
                .Where(b => b.UserId == userId && b.TourId == tourId 
                    && (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed))
                .AnyAsync();

            if (!hasParticipated)
            {
                TempData["ErrorMessage"] = "Bạn cần trải nghiệm tour trước khi đánh giá";
                return RedirectToAction("Details", "Tour", new { id = tourId });
            }

            // Kiểm tra xem user đã viết review cho tour này chưa
            var existingReview = await _context.Reviews
                .Where(r => r.UserId == userId && r.TourId == tourId)
                .FirstOrDefaultAsync();

            if (existingReview != null)
            {
                TempData["InfoMessage"] = "Bạn đã viết đánh giá cho tour này rồi";
                return RedirectToAction("Details", "Tour", new { id = tourId });
            }

            // Tạo review object mới với dữ liệu từ server
            var newReview = new Review
            {
                UserId = userId,
                TourId = tourId,
                Rating = rating,
                Comment = comment,
                ReviewDate = DateTime.Now,
                IsApproved = false
            };

            _context.Reviews.Add(newReview);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đánh giá của bạn đã được gửi. Cảm ơn bạn!";
            return RedirectToAction("Details", "Tour", new { id = tourId });
        }

        // GET: Review/GetApprovedReviews
        public async Task<IActionResult> GetApprovedReviews(int tourId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.TourId == tourId && r.IsApproved)
                .Include(r => r.User)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();

            return PartialView("_ReviewsList", reviews);
        }

        // GET: Review/Edit
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (review.UserId != userId)
            {
                return Forbid();
            }

            return View(review);
        }

        // POST: Review/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            var existingReview = await _context.Reviews.FindAsync(id);
            if (existingReview == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (existingReview.UserId != userId)
            {
                return Forbid();
            }

            existingReview.Rating = review.Rating;
            existingReview.Comment = review.Comment;
            existingReview.ReviewDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Reviews.Update(existingReview);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đánh giá đã được cập nhật!";
                return RedirectToAction("Details", "Tour", new { id = existingReview.TourId });
            }

            return View(review);
        }

        // POST: Review/Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đánh giá." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (review.UserId != userId)
            {
                return Json(new { success = false, message = "Bạn không có quyền xóa đánh giá này." });
            }

            int tourId = review.TourId;
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đánh giá đã bị xóa.", redirectUrl = Url.Action("Details", "Tour", new { id = tourId }) });
        }
    }
}
