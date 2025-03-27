using System.ComponentModel.DataAnnotations;

namespace ACC_Tour.Models
{
    public class DescriptionImage
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập URL hình ảnh")]
        [Display(Name = "URL hình ảnh")]
        public string Url { get; set; }

        public int TourId { get; set; } // Khóa ngoại để liên kết với Tour
        public virtual Tour Tour { get; set; } // Mối quan hệ với Tour
    }
} 