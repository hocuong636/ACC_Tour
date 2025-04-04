using System.ComponentModel.DataAnnotations;
using ACC_Tour.Models;

public class Review
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; }
    
    [Required]
    public int TourId { get; set; }
    
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }
    
    [Required]
    public string Comment { get; set; }
    
    [Required]
    public DateTime ReviewDate { get; set; }
    
    public bool IsApproved { get; set; }
    public string? AdminResponse { get; set; }
    
    [Required]
    public virtual ApplicationUser User { get; set; }
    
    [Required]
    public virtual Tour Tour { get; set; }
}
