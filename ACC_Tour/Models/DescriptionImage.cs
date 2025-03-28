using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ACC_Tour.Models
{
    public class DescriptionImage
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hình ảnh.")]
        public string? ImageFile { get; set; }

        [Display(Name = "Hình ảnh")]
        [NotMapped]
        public List<IFormFile>? ImageUploads { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ID Tour.")]
        public int TourId { get; set; }

        public virtual Tour Tour { get; set; } // Mối quan hệ với Tour
    }
}