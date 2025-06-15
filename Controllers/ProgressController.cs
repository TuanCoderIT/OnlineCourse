using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;

namespace OnlineCourse.Controllers
{
    public class ProgressController : ControllerBase
    {
        private readonly DataContext _context;
        public ProgressController(DataContext context)
        {
            _context = context;
        }
        [HttpGet("api/progress/{courseId}")]
        public async Task<IActionResult> GetProgressAsync([FromRoute] int courseId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var processCount = 0;
                var totalLessons = 0;
                var lessons = await _context.Lessons.Where(p => p.CourseId == courseId).ToListAsync();
                if (!lessons.Any()) totalLessons = 0;
                var processes = await _context.Progresses.Where(p => p.CourseId == courseId && p.UserId == userId).ToListAsync();
                if (!processes.Any()) processCount = 0;
                else
                {
                    processCount = processes.Count;
                }
                return Ok(
                    new
                    {
                        totalLessons = lessons.Count,
                        completedLessons = processCount
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("api/progress/checkLessonComplete/{lessonId}")]
        public async Task<IActionResult> CheckLessonCompleteAsync([FromRoute] int lessonId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var process = await _context.Progresses.FirstOrDefaultAsync(p => p.LessonId == lessonId && p.UserId == userId);
                if (process == null) return Ok(false);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("api/progress/addCompleteLesson/{courseId}/{lessonId}")]
        public async Task<IActionResult> AddCompleteLessonAsync([FromRoute] int courseId, [FromRoute] int lessonId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var process = await _context.Progresses.FirstOrDefaultAsync(p => p.LessonId == lessonId && p.UserId == userId);
                if (process != null) return BadRequest("Lesson already completed");
                var progressModel = new Progresses();
                progressModel.UserId = userId;
                progressModel.IsCompleted = true;
                progressModel.LessonId = lessonId;
                progressModel.CourseId = courseId;
                await _context.Progresses.AddAsync(progressModel);
                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}