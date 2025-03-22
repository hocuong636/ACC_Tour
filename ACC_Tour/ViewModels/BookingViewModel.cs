using System.ComponentModel.DataAnnotations;

namespace ACC_Tour.ViewModels
{
    public class BookingViewModel
    {
        public int TourId { get; set; }

        [Required(ErrorMessage = "Số người tham gia là bắt buộc")]
        [Display(Name = "Số người tham gia")]
        [Range(1, int.MaxValue, ErrorMessage = "Số người phải lớn hơn 0")]
        public int NumberOfParticipants { get; set; }

        [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc")]
        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
