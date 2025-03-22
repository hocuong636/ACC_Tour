using ACC_Tour.Models;

public class Booking
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int TourId { get; set; }
    public DateTime BookingDate { get; set; }
    public int NumberOfParticipants { get; set; }
    public decimal TotalAmount { get; set; }
    public BookingStatus Status { get; set; }
    public string PaymentMethod { get; set; }
    public string PaymentStatus { get; set; }
    public virtual ApplicationUser User { get; set; }
    public virtual Tour Tour { get; set; }
}

public enum BookingStatus
{
    Pending,
    Confirmed,
    Cancelled,
    Completed
}
