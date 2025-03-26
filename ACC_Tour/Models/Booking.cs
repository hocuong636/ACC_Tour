using System.ComponentModel.DataAnnotations;
using ACC_Tour.Models;

namespace ACC_Tour.Models;

public class Booking
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; }
    
    [Required]
    public int TourId { get; set; }
    
    [Required]
    public DateTime BookingDate { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int NumberOfParticipants { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }
    
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

public enum BookingStatus
{
    Pending,
    Confirmed,
    Cancelled,
    Completed
}