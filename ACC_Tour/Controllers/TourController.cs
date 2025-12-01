using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ACC_Tour.Data;
using ACC_Tour.Models;
using Microsoft.AspNetCore.Authorization;
using ACC_Tour.ViewModels;

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
                       where t.IsActive == true
                       select t;

            if (!String.IsNullOrEmpty(searchString))
            {
                tours = tours.Where(t => t.Name.Contains(searchString));
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
                    .ThenInclude(r => r.User)
                .Include(t => t.DescriptionImages)
                .FirstOrDefaultAsync(t => t.Id == id && t.IsActive == true);

            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }

        [Authorize]
        public async Task<IActionResult> Book(int? id)
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

            var bookingViewModel = new BookingViewModel
            {
                TourId = tour.Id,
                TourName = tour.Name,
                Price = tour.Price,
                StartDate = tour.StartDate,
                EndDate = tour.EndDate
            };

            return View(bookingViewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(BookingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var booking = new Booking
                {
                    TourId = model.TourId,
                    UserId = User.Identity.Name,
                    BookingDate = DateTime.Now,
                    NumberOfParticipants = model.NumberOfParticipants,
                    TotalPrice = model.Price * model.NumberOfParticipants,
                    Status = BookingStatus.Pending,
                    PaymentMethod = "Chưa thanh toán",
                    PaymentStatus = "Chờ thanh toán"
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đặt tour thành công!";
                return RedirectToAction(nameof(MyBookings));
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> MyBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Tour)
                .Where(b => b.UserId == User.Identity.Name)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Tour)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == User.Identity.Name);

            if (booking == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn đặt tour." });
            }

            if (booking.Status != BookingStatus.Pending)
            {
                return Json(new { success = false, message = "Chỉ có thể hủy đặt tour khi trạng thái là đang chờ xử lý." });
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

            return Json(new { success = true, message = "Hủy đặt tour thành công!" });
        }
    }
}
