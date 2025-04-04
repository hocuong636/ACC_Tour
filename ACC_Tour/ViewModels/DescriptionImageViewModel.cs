using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ACC_Tour.ViewModels
{
    public class DescriptionImageViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn Tour.")]
        [Display(Name = "Tour")]
        public int TourId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ít nhất một hình ảnh.")]
        [Display(Name = "Hình ảnh")]
        public List<IFormFile> ImageFiles { get; set; }
    }
}