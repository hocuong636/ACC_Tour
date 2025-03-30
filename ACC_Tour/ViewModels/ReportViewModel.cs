using System;
using System.Collections.Generic;

namespace ACC_Tour.ViewModels
{
    public class ReportViewModel
    {
        public decimal MonthlyRevenue { get; set; }
        public int TotalTours { get; set; }
        public int TotalCustomers { get; set; }
        public int NewOrders { get; set; }
        public object MonthlyRevenueData { get; set; }
        public object BookingStatusData { get; set; }
        public object TopTours { get; set; }
        public object RevenueByStatus { get; set; }

        // Thêm các thuộc tính cho bộ lọc
        public DateTime? StartDate { get; set; } = DateTime.Now.AddMonths(-1);
        public DateTime? EndDate { get; set; } = DateTime.Now;
    }
} 