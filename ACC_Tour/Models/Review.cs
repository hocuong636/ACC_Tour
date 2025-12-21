using System.ComponentModel.DataAnnotations;
using ACC_Tour.Models;

public class Review
{
    public int Id { get; set; }
    
    // Được set từ server (claim), không validate từ form
    public string UserId { get; set; }
    
    // Được set từ server (route), không validate từ form  
    public int TourId { get; set; }
    
    [Required(ErrorMessage = "Vui lòng chọn số sao")]
    [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao")]
    public int Rating { get; set; }
    
    [Required(ErrorMessage = "Vui lòng nhập nhận xét")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Nhận xét phải từ 10 đến 1000 ký tự")]
    public string Comment { get; set; }
    
    // Được set từ server (DateTime.Now), không validate từ form
    public DateTime ReviewDate { get; set; }
    
    public bool IsApproved { get; set; }
    public string? AdminResponse { get; set; }
    
    // Các navigation property không require vì sẽ được load sau
    public virtual ApplicationUser User { get; set; }
    
    public virtual Tour Tour { get; set; }
}
