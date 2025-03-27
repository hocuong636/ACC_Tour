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
                MonthlyRevenueData = await GetMonthlyRevenueData()
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
                        Status = b.Status.ToString()
                    })
                    .ToListAsync();

                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy dữ liệu: {ex.Message}");
            }
        }

        private async Task<decimal> CalculateMonthlyRevenue()
        {
            try
            {
                var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                return await _context.Bookings
                    .Where(b => b.BookingDate >= firstDayOfMonth && 
                               b.BookingDate <= lastDayOfMonth &&
                               b.Status == BookingStatus.Completed)
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
                var sixMonthsAgo = DateTime.Now.AddMonths(-6);
                var monthlyRevenue = await _context.Bookings
                    .Where(b => b.BookingDate >= sixMonthsAgo &&
                               b.Status == BookingStatus.Completed)
                    .GroupBy(b => new { b.BookingDate.Year, b.BookingDate.Month })
                    .Select(g => new
                    {
                        Month = $"{g.Key.Month:00}/{g.Key.Year}",
                        Revenue = g.Sum(b => b.TotalPrice)
                    })
                    .OrderBy(x => x.Month)
                    .ToListAsync();

                return new
                {
                    Labels = monthlyRevenue.Select(x => x.Month).ToList(),
                    Data = monthlyRevenue.Select(x => x.Revenue).ToList()
                };
            }
            catch
            {
                return new { Labels = new List<string>(), Data = new List<decimal>() };
            }
        }
    }
}

