using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ACC_Tour.Models;

namespace Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tour> Tours { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Cấu hình kiểu dữ liệu cho các trường decimal
            builder.Entity<Tour>()
                .Property(t => t.Price)
                .HasPrecision(18, 2);  // 18 chữ số tổng cộng, 2 chữ số sau dấu phẩy

            builder.Entity<Booking>()
                .Property(b => b.TotalAmount)
                .HasPrecision(18, 2);  // 18 chữ số tổng cộng, 2 chữ số sau dấu phẩy

            // Cấu hình các relationship và ràng buộc
            builder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.Tour)
                .WithMany(t => t.Bookings)
                .HasForeignKey(b => b.TourId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review>()
                .HasOne(r => r.Tour)
                .WithMany(t => t.Reviews)
                .HasForeignKey(r => r.TourId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
