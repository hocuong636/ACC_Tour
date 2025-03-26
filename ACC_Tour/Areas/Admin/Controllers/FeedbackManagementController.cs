using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ACC_Tour.Data;
using ACC_Tour.Models;

namespace ACC_Tour.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class FeedbackManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedbackManagementController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/FeedbackManagement
        public async Task<IActionResult> Index()
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Tour)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
            return View(feedbacks);
        }

        // GET: Admin/FeedbackManagement/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Tour)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // POST: Admin/FeedbackManagement/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            feedback.IsApproved = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/FeedbackManagement/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            feedback.IsApproved = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/FeedbackManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
} 