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

        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var blogs = await _context.Blogs
                .Include(b => b.Categories)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
            return View(blogs);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _context.BlogCategories.ToListAsync();
            ViewBag.Categories = new MultiSelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog, List<int> categoryIds, IFormFile imageFile)
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

                if (categoryIds != null && categoryIds.Any())
                {
                    blog.Categories = await _context.BlogCategories
                        .Where(c => categoryIds.Contains(c.Id))
                        .ToListAsync();
                }

                _context.Blogs.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var categories = await _context.BlogCategories.ToListAsync();
            ViewBag.Categories = new MultiSelectList(categories, "Id", "Name");
            return View(blog);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var blog = await _context.Blogs
                .Include(b => b.Categories)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null)
            {
                return NotFound();
            }

            var categories = await _context.BlogCategories.ToListAsync();
            var selectedCategoryIds = blog.Categories?.Select(c => c.Id).ToList() ?? new List<int>();
            ViewBag.Categories = new MultiSelectList(categories, "Id", "Name", selectedCategoryIds);
            
            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Blog blog, List<int> categoryIds, IFormFile imageFile)
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
                        .Include(b => b.Categories)
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

                    existingBlog.Title = blog.Title;
                    existingBlog.Content = blog.Content;
                    existingBlog.ShortDescription = blog.ShortDescription;
                    existingBlog.IsPublished = blog.IsPublished;
                    existingBlog.UpdatedAt = DateTime.Now;
                    existingBlog.Author = blog.Author;
                    existingBlog.Slug = blog.Slug;

                    if (categoryIds != null && categoryIds.Any())
                    {
                        existingBlog.Categories = await _context.BlogCategories
                            .Where(c => categoryIds.Contains(c.Id))
                            .ToListAsync();
                    }
                    else
                    {
                        existingBlog.Categories.Clear();
                    }

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

            var categories = await _context.BlogCategories.ToListAsync();
            ViewBag.Categories = new MultiSelectList(categories, "Id", "Name", categoryIds);
            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
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
    }
} 