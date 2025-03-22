using System.ComponentModel.DataAnnotations;

namespace ACC_Tour.ViewModels
{
    public class TourViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên tour là bắt buộc")]
        [Display(Name = "Tên tour")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Mô tả là bắt buộc")]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Display(Name = "Giá")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Số người tối đa là bắt buộc")]
        [Display(Name = "Số người tối đa")]
        [Range(1, int.MaxValue, ErrorMessage = "Số người phải lớn hơn 0")]
        public int MaxParticipants { get; set; }

        [Display(Name = "Hình ảnh")]
        public string ImageUrl { get; set; }

        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; }
    }
}
