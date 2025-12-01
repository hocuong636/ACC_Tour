using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ACC_Tour.Models;
using System.Text.RegularExpressions;
using ACC_Tour.Data;

namespace ACC_Tour.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BlogController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var blogs = await _context.Blogs
                .OrderByDescending(b => b.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
            return View(blogs);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                blog.Slug = GenerateSlug(blog.Title);
                blog.CreatedAt = DateTime.Now;
                
                if (imageFile != null)
                {
                    // Xử lý upload hình ảnh
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "blogs", uniqueFileName);
                    
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    
                    blog.ImageUrl = $"/uploads/blogs/{uniqueFileName}";
                }

                // Xử lý nội dung từ editor
                blog.Content = await ProcessEditorContent(blog.Content);

                _context.Blogs.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(blog);
        }

        public async Task<IActionResult> Details(int id)
        {
            var blog = await _context.Blogs
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var blog = await _context.Blogs
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null)
            {
                return NotFound();
            }
            
            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Blog blog, IFormFile imageFile)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingBlog = await _context.Blogs
                        .FirstOrDefaultAsync(b => b.Id == id);

                    if (existingBlog == null)
                    {
                        return NotFound();
                    }

                    if (imageFile != null)
                    {
                        // Xử lý upload hình ảnh mới
                        var fileName = Path.GetFileName(imageFile.FileName);
                        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "blogs", uniqueFileName);
                        
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        
                        // Xóa file cũ nếu có
                        if (!string.IsNullOrEmpty(existingBlog.ImageUrl))
                        {
                            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                                existingBlog.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }
                        
                        existingBlog.ImageUrl = $"/uploads/blogs/{uniqueFileName}";
                    }

                    // Xử lý nội dung từ editor
                    blog.Content = await ProcessEditorContent(blog.Content);

                    existingBlog.Title = blog.Title;
                    existingBlog.Content = blog.Content;
                    existingBlog.ShortDescription = blog.ShortDescription;
                    existingBlog.IsPublished = blog.IsPublished;
                    existingBlog.UpdatedAt = DateTime.Now;
                    existingBlog.Author = blog.Author;
                    existingBlog.Slug = blog.Slug;

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _context.Blogs
                .Include(b => b.Attachments)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog != null)
            {
                // Xóa file hình ảnh nếu có
                if (!string.IsNullOrEmpty(blog.ImageUrl))
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", 
                        blog.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                // Xóa các file đính kèm
                foreach (var attachment in blog.Attachments)
                {
                    var attachmentPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", 
                        attachment.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(attachmentPath))
                    {
                        System.IO.File.Delete(attachmentPath);
                    }
                }

                _context.Blogs.Remove(blog);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }

        private string GenerateSlug(string title)
        {
            string str = title.ToLowerInvariant();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-");
            return str;
        }

        private async Task<string> ProcessEditorContent(string content)
        {
            // Tạo thư mục uploads nếu chưa tồn tại
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "blogs", "content");
            Directory.CreateDirectory(uploadsFolder);

            // Xử lý các file trong nội dung
            var processedContent = content;
            var base64Images = Regex.Matches(content, @"data:image/(?<type>.+?);base64,(?<data>.+?)");
            var videoUrls = Regex.Matches(content, @"<video[^>]*src=""(?<url>[^""]+)""[^>]*>");

            foreach (Match match in base64Images)
            {
                var imageType = match.Groups["type"].Value;
                var imageData = match.Groups["data"].Value;
                var imageBytes = Convert.FromBase64String(imageData);
                var fileName = $"{Guid.NewGuid()}.{imageType}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
                var relativePath = $"/uploads/blogs/content/{fileName}";
                processedContent = processedContent.Replace(match.Value, relativePath);
            }

            foreach (Match match in videoUrls)
            {
                var videoUrl = match.Groups["url"].Value;
                if (!videoUrl.StartsWith("http"))
                {
                    var videoFileName = Path.GetFileName(videoUrl);
                    var videoFilePath = Path.Combine(uploadsFolder, videoFileName);
                    var relativePath = $"/uploads/blogs/content/{videoFileName}";
                    processedContent = processedContent.Replace(videoUrl, relativePath);
                }
            }

            return processedContent;
        }
    }
} 