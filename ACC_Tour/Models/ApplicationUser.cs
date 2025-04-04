using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using ACC_Tour.Models;

namespace ACC_Tour.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

        // Computed property - không lưu vào database
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
