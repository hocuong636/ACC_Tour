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
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số người tối đa")]
        [Display(Name = "Số người tối đa")]
        [Range(1, int.MaxValue, ErrorMessage = "Số người phải lớn hơn 0")]
        public int MaxParticipants { get; set; }

        [Display(Name = "URL hình ảnh")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
