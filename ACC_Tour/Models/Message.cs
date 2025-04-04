using System.ComponentModel.DataAnnotations;
using ACC_Tour.Models;

public class Message
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; }
    
    [Required]
    public string Subject { get; set; }
    
    [Required]
    public string Content { get; set; }
    
    [Required]
    public DateTime SendDate { get; set; }
    
    public bool IsRead { get; set; }
    public string? AdminResponse { get; set; }
    public DateTime? ResponseDate { get; set; }
    
    [Required]
    public virtual ApplicationUser User { get; set; }
}
