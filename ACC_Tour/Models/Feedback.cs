using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACC_Tour.Models
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        [Display(Name = "Tiêu đề")]
        [StringLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        [Display(Name = "Nội dung")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn đánh giá")]
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao")]
        [Display(Name = "Đánh giá")]
        public int Rating { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Trạng thái")]
        public bool IsApproved { get; set; } = false;

        // Foreign key for User
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        // Optional: If feedback is related to a specific tour
        [Display(Name = "Tour")]
        public int? TourId { get; set; }

        [ForeignKey("TourId")]
        public virtual Tour Tour { get; set; }
    }
} 