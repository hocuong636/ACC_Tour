using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ACC_Tour.Data;
using ACC_Tour.Models;
using System.Security.Claims;

namespace ACC_Tour.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Booking
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookings = await _context.Bookings
                .Include(b => b.Tour)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }

        // GET: Booking/Create/5
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours
                .FirstOrDefaultAsync(t => t.Id == id && t.IsActive == true);

            if (tour == null)
            {
                TempData["ErrorMessage"] = "Tour không tồn tại hoặc không khả dụng.";
                return RedirectToAction("Index", "Tour");
            }

            // Kiểm tra tour đã qua ngày khởi hành chưa
            if (tour.StartDate.Date <= DateTime.Now.Date)
            {
                TempData["ErrorMessage"] = "Không thể đặt tour đã bắt đầu hoặc đã qua ngày khởi hành.";
                return RedirectToAction("Index", "Tour");
            }

            // Kiểm tra còn slot không
            if (tour.RemainingSlots <= 0)
            {
                TempData["ErrorMessage"] = "Tour này đã hết chỗ.";
                return RedirectToAction("Index", "Tour");
            }

            var booking = new Booking
            {
                TourId = tour.Id,
                Tour = tour
            };

            return View(booking);
        }

        // POST: Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int tourId, int numberOfParticipants, string paymentMethod, decimal totalPrice)
        {
            var tour = await _context.Tours.FindAsync(tourId);
            if (tour == null)
            {
                return NotFound();
            }

            // Kiểm tra tour có đang active không
            if (!tour.IsActive)
            {
                TempData["ErrorMessage"] = "Tour này hiện không khả dụng để đặt.";
                return RedirectToAction("Index", "Tour");
            }

            // Kiểm tra tour đã qua ngày khởi hành chưa
            if (tour.StartDate.Date <= DateTime.Now.Date)
            {
                TempData["ErrorMessage"] = "Không thể đặt tour đã bắt đầu hoặc đã qua ngày khởi hành.";
                return RedirectToAction("Index", "Tour");
            }

            // Kiểm tra số người tối thiểu
            if (numberOfParticipants < tour.MinParticipants)
            {
                TempData["ErrorMessage"] = $"Số người tham gia tối thiểu là {tour.MinParticipants} người.";
                return RedirectToAction("Create", new { id = tourId });
            }

            // Kiểm tra số lượng người tham gia (tối đa)
            if (numberOfParticipants <= 0 || numberOfParticipants > tour.RemainingSlots)
            {
                TempData["ErrorMessage"] = numberOfParticipants > tour.RemainingSlots 
                    ? $"Số người tham gia vượt quá số slot còn lại ({tour.RemainingSlots} người)."
                    : "Số người tham gia phải lớn hơn 0.";
                return RedirectToAction("Create", new { id = tourId });
            }

            // Kiểm tra tổng tiền có đúng không
            decimal calculatedTotal = tour.Price * numberOfParticipants;
            if (totalPrice != calculatedTotal)
            {
                ModelState.AddModelError("TotalPrice", "Tổng tiền không hợp lệ");
                return RedirectToAction("Create", new { id = tourId });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var booking = new Booking
            {
                TourId = tourId,
                UserId = userId,
                BookingDate = DateTime.Now,
                NumberOfParticipants = numberOfParticipants,
                TotalPrice = calculatedTotal,
                Status = BookingStatus.Pending,
                PaymentMethod = paymentMethod,
                PaymentStatus = "Chờ thanh toán",
            };
            // Cập nhật số lượng slot còn lại của tour
            tour.RemainingSlots -= numberOfParticipants;
            // Tour chỉ active khi còn slot VÀ chưa qua ngày khởi hành
            tour.IsActive = tour.RemainingSlots > 0 && tour.StartDate.Date > DateTime.Now.Date;
            _context.Update(tour);

            _context.Add(booking);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đặt tour thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Booking/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var booking = await _context.Bookings
                .Include(b => b.Tour)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Booking/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var booking = await _context.Bookings
                .Include(b => b.Tour)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            if (booking.Status != BookingStatus.Pending)
            {
                TempData["ErrorMessage"] = "Chỉ có thể chỉnh sửa khi đơn đặt tour đang ở trạng thái chờ xử lý.";
                return RedirectToAction(nameof(Details), new { id = booking.Id });
            }

            return View(booking);
        }

        // POST: Booking/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookingStatus status)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            booking.Status = status;
            _context.Update(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Booking/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var booking = await _context.Bookings
                .Include(b => b.Tour)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            if (booking.Status != BookingStatus.Pending)
            {
                TempData["ErrorMessage"] = "Chỉ có thể hủy đặt tour khi trạng thái là đang chờ xử lý.";
                return RedirectToAction(nameof(Index));
            }

            // Cập nhật RemainingSlots khi hủy booking
            var tour = booking.Tour;
            if (tour != null)
            {
                tour.RemainingSlots += booking.NumberOfParticipants;
                // Nếu tour đang không active và có slot trống, kích hoạt lại tour
                if (!tour.IsActive && tour.RemainingSlots > 0)
                {
                    tour.IsActive = true;
                }
                _context.Update(tour);
            }

            booking.Status = BookingStatus.Cancelled;
            booking.PaymentStatus = "Đã hủy";
            _context.Update(booking);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Hủy đặt tour thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}