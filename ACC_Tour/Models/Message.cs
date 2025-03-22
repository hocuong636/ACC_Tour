using ACC_Tour.Models;

public class Message
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
    public DateTime SendDate { get; set; }
    public bool IsRead { get; set; }
    public string AdminResponse { get; set; }
    public DateTime? ResponseDate { get; set; }
    public virtual ApplicationUser User { get; set; }
}
