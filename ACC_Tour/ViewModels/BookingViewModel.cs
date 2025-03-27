using System.ComponentModel.DataAnnotations;

namespace ACC_Tour.ViewModels
{
    public class BookingViewModel
    {
        public int TourId { get; set; }
        
        [Display(Name = "Tên tour")]
        public string TourName { get; set; }

        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số người tham gia")]
        [Display(Name = "Số người tham gia")]
        [Range(1, 100, ErrorMessage = "Số người tham gia phải từ 1 đến 100")]
        public int NumberOfParticipants { get; set; }

        [Display(Name = "Tổng tiền")]
        public decimal TotalPrice => Price * NumberOfParticipants;

        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; } = "Chưa thanh toán";

        [Display(Name = "Trạng thái thanh toán")]
        public string PaymentStatus { get; set; } = "Chờ thanh toán";
    }
}
