using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACC_Tour.Models
{
    public class TourAssignment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tour")]
        [Display(Name = "Tour")]
        public int TourId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hướng dẫn viên")]
        [Display(Name = "Hướng dẫn viên")]
        public int TourGuideId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(TourAssignment), "ValidateStartDate")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(TourAssignment), "ValidateEndDate")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Trạng thái")]
        public AssignmentStatus Status { get; set; } = AssignmentStatus.Pending;

        [Display(Name = "Ghi chú")]
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("TourId")]
        public Tour Tour { get; set; }

        [ForeignKey("TourGuideId")]
        public TourGuide TourGuide { get; set; }

        public static ValidationResult ValidateStartDate(DateTime startDate, ValidationContext context)
        {
            var assignment = (TourAssignment)context.ObjectInstance;
            if (startDate < DateTime.Today)
            {
                return new ValidationResult("Ngày bắt đầu không thể là ngày trong quá khứ");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult ValidateEndDate(DateTime endDate, ValidationContext context)
        {
            var assignment = (TourAssignment)context.ObjectInstance;
            if (endDate <= assignment.StartDate)
            {
                return new ValidationResult("Ngày kết thúc phải sau ngày bắt đầu");
            }
            return ValidationResult.Success;
        }
    }

    public enum AssignmentStatus
    {
        [Display(Name = "Chờ xác nhận")]
        Pending,
        [Display(Name = "Đã xác nhận")]
        Confirmed,
        [Display(Name = "Đã hoàn thành")]
        Completed,
        [Display(Name = "Đã hủy")]
        Cancelled
    }
} 