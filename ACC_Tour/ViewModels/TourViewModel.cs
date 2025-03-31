using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACC_Tour.ViewModels
{
    public class TourViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên tour")]
        [Display(Name = "Tên Tour")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá tour")]
        [Display(Name = "Giá (VNĐ)")]
        [Range(1000, 1000000000, ErrorMessage = "Giá tour phải từ 1,000 đến 1,000,000,000 VNĐ")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,0)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(TourViewModel), "ValidateEndDate")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số người tối đa")]
        [Display(Name = "Số người tối đa")]
        [Range(1, int.MaxValue, ErrorMessage = "Số người phải lớn hơn 0")]
        public int MaxParticipants { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số người tối thiểu")]
        [Display(Name = "Số người tối thiểu")]
        [Range(1, int.MaxValue, ErrorMessage = "Số người tối thiểu phải lớn hơn 0")]
        public int MinParticipants { get; set; }

        [Display(Name = "Hình ảnh hiện tại")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Hình ảnh mới")]
        [DataType(DataType.Upload)]
        public IFormFile? ImageFile { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả tour")]
        [Display(Name = "Mô tả")]
        [MinLength(50, ErrorMessage = "Mô tả phải có ít nhất 50 ký tự")]
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public static ValidationResult ValidateEndDate(DateTime endDate, ValidationContext context)
        {
            var tourViewModel = (TourViewModel)context.ObjectInstance;
            if (endDate <= tourViewModel.StartDate)
            {
                return new ValidationResult("Ngày kết thúc phải sau ngày bắt đầu");
            }
            return ValidationResult.Success;
        }
    }
}
