using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;
using OnlineCourse.Models.ViewModels;

namespace OnlineCourse.Controllers
{
    public class CourseController : Controller
    {
        private readonly DataContext _context;
        public CourseController(DataContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var course = await _context.Courses
                .Include(c => c.Chapters)
                .ThenInclude(c => c.Lessons)
                .Include(c => c.Ratings)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            var totalRatings = course.Ratings.Count;

            var starStats = new Dictionary<int, int>();
            for (int star = 5; star >= 1; star--)
            {
                starStats[star] = course.Ratings.Count(r => r.Rate == star);
            }

            var starPercentage = starStats.ToDictionary(
                k => k.Key,
                k => totalRatings > 0 ? (k.Value * 100) / totalRatings : 0
            );

            ViewBag.StarStats = starStats;      
            ViewBag.StarPercentage = starPercentage;

            var averageRate = course.Ratings.Count > 0 ? Math.Round(course.Ratings.Average(r => r.Rate), 1) : 0;
            ViewBag.AverageRate = averageRate;

            return View(course);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRating(int courseId, string comment, int rate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var rating = new Rating
            {
                CourseId = courseId,
                UserId = userId,
                Comment = comment,
                Rate = rate,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now 
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Course", new { id = courseId });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditRating(int id)
        {
            var review = await _context.Ratings.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (review.UserId != currentUserId)
            {
                return Forbid();
            }

            return View(review);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRating(Rating rating)
        {
            var existingRating = await _context.Ratings.FindAsync(rating.Id);
            if (existingRating == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (existingRating.UserId != currentUserId)
            {
                return Forbid();
            }

            existingRating.Comment = rating.Comment;
            existingRating.Rate = rating.Rate;
            existingRating.UpdatedAt = DateTime.Now;

            _context.Ratings.Update(existingRating);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Course", new { id = existingRating.CourseId });
        }

        [Authorize]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRating(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var review = await _context.Ratings.FindAsync(id);

            if (review == null || review.UserId != userId)
            {
                return Forbid();
            }

            _context.Ratings.Remove(review);
            await _context.SaveChangesAsync();

            return Ok();
        }

        public async Task<IActionResult> Search(string name)
        {
            var courses = await _context.Courses
                .Where(c => c.Name.Contains(name))
                .ToListAsync();
            ViewBag.Keyword = name;

            return View(courses);
        }

    }   
}