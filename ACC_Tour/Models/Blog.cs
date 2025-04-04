using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACC_Tour.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống")]
        public string Content { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả ngắn không được vượt quá 500 ký tự")]
        public string? ShortDescription { get; set; }

        [StringLength(200)]
        public string? ImageUrl { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        [Required]
        public bool IsPublished { get; set; } = false;

        [StringLength(100)]
        public string? Author { get; set; }

        [StringLength(100)]
        public string? Slug { get; set; }

        public List<BlogCategory> Categories { get; set; } = new();

        public List<BlogAttachment> Attachments { get; set; } = new();
    }

    public class BlogAttachment
    {
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FilePath { get; set; }

        [Required]
        public string FileType { get; set; } // image, video, etc.

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }

    public class BlogCategory
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
        public string Name { get; set; }

        [StringLength(200)]
        public string? Slug { get; set; }

        public string? Description { get; set; }

        public List<Blog> Blogs { get; set; } = new();
    }
} 