using System.ComponentModel.DataAnnotations;

namespace ACC_Tour.Models
{
    public class TourGuide
    {
        public TourGuide()
        {
            TourAssignments = new List<TourAssignment>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên hướng dẫn viên")]
        [Display(Name = "Tên hướng dẫn viên")]
        [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Số điện thoại phải có 10 chữ số")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số năm kinh nghiệm")]
        [Display(Name = "Kinh nghiệm")]
        [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm phải từ 0 đến 50")]
        public int Experience { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngôn ngữ")]
        [Display(Name = "Ngôn ngữ")]
        [StringLength(200, ErrorMessage = "Ngôn ngữ không được vượt quá 200 ký tự")]
        public string Languages { get; set; }

        [Display(Name = "Mô tả")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }

        // Navigation property
        public virtual ICollection<TourAssignment> TourAssignments { get; set; }
    }
} 