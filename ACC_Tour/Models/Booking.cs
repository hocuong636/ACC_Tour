using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ACC_Tour.Models;

namespace ACC_Tour.Models;

public enum BookingStatus
{
    Pending,
    Confirmed,
    Cancelled,
    Completed
}

public class Booking
{
    public int Id { get; set; }
    
    [Required]
    public int TourId { get; set; }
    
    [Required]
    public string UserId { get; set; }
    
    [Required]
    public DateTime BookingDate { get; set; }
    
    [Required]
    [Range(1, 100)]
    public int NumberOfParticipants { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }
    
    [Required]
    public BookingStatus Status { get; set; }
    
    [Required]
    public string PaymentMethod { get; set; }
    
    [Required]
    public string PaymentStatus { get; set; }
    
    [Required]
    public virtual ApplicationUser User { get; set; }
    
    [Required]
    public virtual Tour Tour { get; set; }
}