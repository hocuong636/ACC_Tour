using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ACC_Tour.Data;
using ACC_Tour.Models;
using Microsoft.AspNetCore.Identity;

namespace ACC_Tour.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FeedbackController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Feedback
        public async Task<IActionResult> Index()
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Tour)
                .Where(f => f.IsApproved)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
            return View(feedbacks);
        }

        // GET: Feedback/Create
        [Authorize]
        public IActionResult Create(int? tourId)
        {
            ViewBag.TourId = tourId;
            return View();
        }

        // POST: Feedback/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,Rating,TourId")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                feedback.UserId = _userManager.GetUserId(User);
                feedback.CreatedAt = DateTime.Now;
                feedback.IsApproved = false;

                _context.Add(feedback);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyFeedbacks));
            }
            return View(feedback);
        }

        // GET: Feedback/MyFeedbacks
        [Authorize]
        public async Task<IActionResult> MyFeedbacks()
        {
            var userId = _userManager.GetUserId(User);
            var feedbacks = await _context.Feedbacks
                .Include(f => f.Tour)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
            return View(feedbacks);
        }

        // GET: Feedback/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (feedback.UserId != userId)
            {
                return Forbid();
            }

            return View(feedback);
        }

        // POST: Feedback/Edit/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,Rating")] Feedback feedback)
        {
            if (id != feedback.Id)
            {
                return NotFound();
            }

            var existingFeedback = await _context.Feedbacks.FindAsync(id);
            if (existingFeedback == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (existingFeedback.UserId != userId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingFeedback.Title = feedback.Title;
                    existingFeedback.Content = feedback.Content;
                    existingFeedback.Rating = feedback.Rating;
                    existingFeedback.IsApproved = false;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedbackExists(feedback.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MyFeedbacks));
            }
            return View(feedback);
        }

        // POST: Feedback/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (feedback.UserId != userId)
            {
                return Forbid();
            }

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyFeedbacks));
        }

        private bool FeedbackExists(int id)
        {
            return _context.Feedbacks.Any(e => e.Id == id);
        }
    }
}