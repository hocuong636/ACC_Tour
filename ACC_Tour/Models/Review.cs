using ACC_Tour.Models;

public class Review
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int TourId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTime ReviewDate { get; set; }
    public bool IsApproved { get; set; }
    public string AdminResponse { get; set; }
    public virtual ApplicationUser User { get; set; }
    public virtual Tour Tour { get; set; }
}
