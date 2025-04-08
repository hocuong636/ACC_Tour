using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACC_Tour.Models
{
    public class TourAssignment
    {
        public int Id { get; set; }

        [Display(Name = "Tour")]
        public int TourId { get; set; }

        [Display(Name = "Hướng dẫn viên")]
        public int TourGuideId { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Trạng thái")]
        public AssignmentStatus Status { get; set; } = AssignmentStatus.Pending;

        [Display(Name = "Ghi chú")]
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("TourId")]
        [NotMapped]
        public Tour Tour { get; set; }

        [ForeignKey("TourGuideId")]
        [NotMapped]
        public TourGuide TourGuide { get; set; }
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