using ACC_Tour.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ACC_Tour.Data
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
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<DescriptionImage> DescriptionImages { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Cấu hình kiểu dữ liệu cho các trường decimal
            builder.Entity<Tour>()
                .Property(t => t.Price)
                .HasPrecision(18, 2);  // 18 chữ số tổng cộng, 2 chữ số sau dấu phẩy

            builder.Entity<Booking>()
                .Property(b => b.TotalPrice)
                .HasPrecision(18, 2);  // 18 chữ số tổng cộng, 2 chữ số sau dấu phẩy

            // Cấu hình các relationship và ràng buộc cho Booking
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

            // Cấu hình các relationship và ràng buộc cho Review
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

            // Cấu hình relationship cho Message
            builder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ nhiều-nhiều giữa Blog và BlogCategory
            builder.Entity<Blog>()
                .HasMany(b => b.Categories)
                .WithMany(c => c.Blogs);

            builder.Entity<Tour>()
                .HasMany(t => t.DescriptionImages)
                .WithOne(di => di.Tour)
                .HasForeignKey(di => di.TourId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}