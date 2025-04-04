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
                return NotFound();
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

            // Kiểm tra số lượng người tham gia
            if (numberOfParticipants <= 0 || numberOfParticipants > tour.RemainingSlots)
            {
                ModelState.AddModelError("NumberOfParticipants", "Số người tham gia không hợp lệ");
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
            tour.IsActive = tour.RemainingSlots > 0; // Nếu còn slot thì tour vẫn hoạt động, ngược lại thì không
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

            booking.Status = BookingStatus.Cancelled;
            _context.Update(booking);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Hủy đặt tour thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}