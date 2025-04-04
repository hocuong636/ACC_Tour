using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ACC_Tour.Models
{
    public class Tour
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên tour")]
        [Display(Name = "Tên Tour")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá tour")]
        [Display(Name = "Giá")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(Tour), "ValidateEndDate")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số người tối đa")]
        [Display(Name = "Số người tối đa")]
        [Range(1, int.MaxValue, ErrorMessage = "Số người phải lớn hơn 0")]
        public int MaxParticipants { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số người tối thiểu")]
        [Display(Name = "Số người tối thiểu")]
        [Range(1, int.MaxValue, ErrorMessage = "Số người tối thiểu phải lớn hơn 0")]
        public int MinParticipants { get; set; }

        [Display(Name = "Slot còn lại")]
        public int RemainingSlots { get; set; }

        [Display(Name = "Trạng thái tour")]
        public string? Status { get; set; }

        [Display(Name = "URL hình ảnh")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Hình ảnh mô tả")]
        public virtual ICollection<DescriptionImage> DescriptionImages { get; set; } = new List<DescriptionImage>();

        public bool IsActive { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

        public static ValidationResult ValidateEndDate(DateTime endDate, ValidationContext context)
        {
            var tour = (Tour)context.ObjectInstance;
            if (endDate <= tour.StartDate)
            {
                return new ValidationResult("Ngày kết thúc phải sau ngày bắt đầu");
            }
            return ValidationResult.Success;
        }
    }
}
