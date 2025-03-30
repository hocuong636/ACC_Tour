using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ACC_Tour.Models;
using ACC_Tour.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using ACC_Tour.Data;

namespace ACC_Tour.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Report
        public async Task<IActionResult> Index()
        {
            var viewModel = new ReportViewModel
            {
                MonthlyRevenue = await CalculateMonthlyRevenue(),
                TotalTours = await CalculateTotalTours(),
                TotalCustomers = await CalculateTotalCustomers(),
                NewOrders = await CalculateNewOrders(),
                MonthlyRevenueData = await GetMonthlyRevenueData(),
                BookingStatusData = await GetBookingStatusData(),
                TopTours = await GetTopToursData(),
                RevenueByStatus = await GetRevenueByStatus()
            };

            return View(viewModel);
        }

        // API: Lấy doanh thu theo khoảng thời gian
        [HttpGet]
        public async Task<IActionResult> GetRevenueByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest("Ngày bắt đầu phải nhỏ hơn ngày kết thúc");
                }

                // Đảm bảo lấy đủ dữ liệu của cả ngày
                endDate = endDate.AddDays(1).AddTicks(-1);

                var result = await _context.Bookings
                    .Include(b => b.Tour)
                    .Where(b => b.BookingDate >= startDate && b.BookingDate <= endDate)
                    .OrderByDescending(b => b.BookingDate)
                    .Select(b => new
                    {
                        Time = b.BookingDate.ToString("dd/MM/yyyy"),
                        TourCode = b.Tour.Id,
                        TourName = b.Tour.Name,
                        NumberOfCustomers = b.NumberOfParticipants,
                        Revenue = b.TotalPrice,
                        Status = b.Status.ToString(),
                        PaymentStatus = b.PaymentStatus
                    })
                    .ToListAsync();

                var completedBookings = result.Where(r => r.Status == "Completed" || r.PaymentStatus == "Đã thanh toán");

                var summary = new
                {
                    TotalRevenue = completedBookings.Sum(r => r.Revenue),
                    TotalBookings = result.Count,
                    TotalCustomers = result.Sum(r => r.NumberOfCustomers),
                    CompletedBookings = completedBookings.Count(),
                    Data = result
                };

                return Json(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy dữ liệu: {ex.Message}");
            }
        }

        // API: Lấy thống kê theo tour
        [HttpGet]
        public async Task<IActionResult> GetTourStatistics(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.Bookings.Include(b => b.Tour).AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(b => b.BookingDate >= startDate);
                if (endDate.HasValue)
                    query = query.Where(b => b.BookingDate <= endDate);

                var result = await query
                    .GroupBy(b => new { b.Tour.Id, b.Tour.Name })
                    .Select(g => new
                    {
                        TourId = g.Key.Id,
                        TourName = g.Key.Name,
                        TotalBookings = g.Count(),
                        TotalRevenue = g.Sum(b => b.TotalPrice),
                        TotalCustomers = g.Sum(b => b.NumberOfParticipants)
                    })
                    .OrderByDescending(x => x.TotalRevenue)
                    .ToListAsync();

                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy thống kê tour: {ex.Message}");
            }
        }

        private async Task<decimal> CalculateMonthlyRevenue()
        {
            try
            {
                var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddTicks(-1);

                return await _context.Bookings
                    .Where(b => b.BookingDate >= firstDayOfMonth && 
                               b.BookingDate <= lastDayOfMonth &&
                               (b.Status == BookingStatus.Completed || b.PaymentStatus == "Đã thanh toán"))
                    .SumAsync(b => b.TotalPrice);
            }
            catch   
            {
                return 0;
            }
        }

        private async Task<int> CalculateTotalTours()
        {
            return await _context.Tours.CountAsync(t => t.IsActive);
        }

        private async Task<int> CalculateTotalCustomers()
        {
            return await _context.Users.CountAsync();
        }

        private async Task<int> CalculateNewOrders()
        {
            var today = DateTime.Today;
            return await _context.Bookings
                .CountAsync(b => b.BookingDate.Date == today);
        }

        private async Task<object> GetMonthlyRevenueData()
        {
            try
            {
                // Lấy ngày đầu tiên của 6 tháng trước
                var today = DateTime.Today;
                var firstDayOfCurrentMonth = new DateTime(today.Year, today.Month, 1);
                var sixMonthsAgo = firstDayOfCurrentMonth.AddMonths(-5);

                // Lấy dữ liệu doanh thu từ database
                var monthlyRevenue = await _context.Bookings
                    .Where(b => b.BookingDate >= sixMonthsAgo && 
                               b.BookingDate < firstDayOfCurrentMonth.AddMonths(1) &&
                               (b.Status == BookingStatus.Completed || b.PaymentStatus == "Đã thanh toán"))
                    .GroupBy(b => new { 
                        Year = b.BookingDate.Year, 
                        Month = b.BookingDate.Month 
                    })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Revenue = g.Sum(b => b.TotalPrice)
                    })
                    .ToListAsync();

                // Tạo danh sách đầy đủ 6 tháng
                var allMonths = Enumerable.Range(0, 6)
                    .Select(i => sixMonthsAgo.AddMonths(i))
                    .Select(date => new
                    {
                        Year = date.Year,
                        Month = date.Month,
                        MonthName = date.ToString("MM/yyyy"),
                        Revenue = monthlyRevenue
                            .FirstOrDefault(r => r.Year == date.Year && r.Month == date.Month)
                            ?.Revenue ?? 0
                    })
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.Month)
                    .ToList();

                return new
                {
                    Labels = allMonths.Select(x => x.MonthName).ToList(),
                    Data = allMonths.Select(x => x.Revenue).ToList()
                };
            }
            catch (Exception ex)
            {
                // Log exception
                return new { Labels = new List<string>(), Data = new List<decimal>() };
            }
        }

        private async Task<object> GetBookingStatusData()
        {
            try
            {
                var statusData = await _context.Bookings
                    .GroupBy(b => b.Status)
                    .Select(g => new
                    {
                        Status = g.Key.ToString(),
                        Count = g.Count()
                    })
                    .ToListAsync();

                return new
                {
                    Labels = statusData.Select(x => x.Status).ToList(),
                    Data = statusData.Select(x => x.Count).ToList()
                };
            }
            catch
            {
                return new { Labels = new List<string>(), Data = new List<int>() };
            }
        }

        private async Task<object> GetTopToursData()
        {
            try
            {
                var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                var topTours = await _context.Bookings
                    .Include(b => b.Tour)
                    .Where(b => (b.Status == BookingStatus.Completed || b.PaymentStatus == "Đã thanh toán"))
                    .GroupBy(b => new { b.Tour.Id, b.Tour.Name })
                    .Select(g => new
                    {
                        tourName = g.Key.Name,
                        totalBookings = g.Count(),
                        totalRevenue = g.Sum(b => b.TotalPrice),
                        totalCustomers = g.Sum(b => b.NumberOfParticipants),
                        averageRevenue = g.Sum(b => b.TotalPrice) / g.Count()
                    })
                    .OrderByDescending(x => x.totalRevenue)
                    .Take(5)
                    .ToListAsync();

                return topTours;
            }
            catch (Exception ex)
            {
                // Log exception
                return new List<object>();
            }
        }

        private async Task<object> GetRevenueByStatus()
        {
            try
            {
                var revenueByStatus = await _context.Bookings
                    .GroupBy(b => b.Status)
                    .Select(g => new
                    {
                        Status = g.Key.ToString(),
                        Revenue = g.Sum(b => b.TotalPrice)
                    })
                    .ToListAsync();

                return new
                {
                    Labels = revenueByStatus.Select(x => x.Status).ToList(),
                    Data = revenueByStatus.Select(x => x.Revenue).ToList()
                };
            }
            catch
            {
                return new { Labels = new List<string>(), Data = new List<decimal>() };
            }
        }
    }
}

