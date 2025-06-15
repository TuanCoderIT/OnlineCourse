using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;

namespace OnlineCourse.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Lesson")]
    [Authorize(Roles = "Admin")]
    public class LessonController : Controller
    {
        private readonly DataContext _context;
        public LessonController(DataContext context)
        {
            _context = context;
        }

        #region Index
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Lessons
                .Include(l => l.Chapter)
                .Include(l => l.Course)
                .ToListAsync());
        }

        [Route("Index/{chapterId}")]
        public async Task<IActionResult> Index(int? chapterId)
        {
            // if (courseId == null)
            // {
            //     return View(await _context.Lessons.Where(x => x.Status == true).ToListAsync());
            // }
            
            var lessons = await _context.Lessons
                .Where(l => l.ChapterId == chapterId)
                .Include(l => l.Chapter)
                .Include(l => l.Course)
                .ToListAsync();

            return View(lessons);
        }
        #endregion Index

        #region Create
        [HttpGet]
        [Route("Create")]
        public IActionResult Create(int chapterId)
        {
            var chapter = _context.Chapters
                .Include(c => c.Course) 
                .FirstOrDefault(c => c.ChapterId == chapterId);

            if (chapter == null)
            {
                ModelState.AddModelError("", "The specified chapter does not exist.");
                return View();
            }

            var lesson = new Lesson
            {
                ChapterId = chapterId,
                CourseId = chapter.CourseId
            };

            return View(lesson);
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Lesson lesson)
        {
            var chapter = _context.Chapters
                .Include(c => c.Course)
                .FirstOrDefault(c => c.ChapterId == lesson.ChapterId);

            if (chapter == null)
            {
                ModelState.AddModelError("", "The specified chapter does not exist.");
                return View(lesson);
            }

            if (chapter.Course == null)
            {
                ModelState.AddModelError("", "The specified course does not exist for this chapter.");
                return View(lesson);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var lastLesson = _context.Lessons
                        .Where(l => l.ChapterId == lesson.ChapterId)
                        .OrderByDescending(l => l.Order)
                        .FirstOrDefault();

                    lesson.Order = lastLesson != null ? lastLesson.Order + 1 : 1;

                    lesson.CourseId = chapter.CourseId;

                    _context.Add(lesson);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Bài học đã được tạo thành công.";

                    return RedirectToAction("Index", new { chapterId = lesson.ChapterId });
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return View(lesson);
        }

        #endregion Create

        #region Edit
        [HttpGet]
        [Route("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }
            
            return View(lesson);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id:int}"), ActionName("Edit")]
        public async Task<IActionResult> Edit(int id, Lesson lesson)
        {
            Console.WriteLine($"Loaded Lesson ID: {id}");
            Console.WriteLine($"Received Lesson ID from form: {lesson.LessonId}");
            Console.WriteLine($"CourseID from form: {lesson.ChapterId}");

            if (id != lesson.LessonId)
            {
                Console.WriteLine($"ID mismatch: {id} != {lesson.LessonId}");
                return BadRequest("Invalid Lesson ID.");
            }

            var existingLesson = await _context.Lessons.FindAsync(id);
            if (existingLesson == null)
            {
                Console.WriteLine("Lesson not found in database.");
                return NotFound("Lesson not found.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingLesson.Name = lesson.Name;
                    existingLesson.Details = lesson.Details;
                    existingLesson.ContentType = lesson.ContentType;
                    var isDuplicateOrder = _context.Lessons
                        .Any(l => l.ChapterId == lesson.ChapterId && l.Order == lesson.Order && l.LessonId != id);
                    
                    if (isDuplicateOrder)
                    {
                        ModelState.AddModelError("", "Thứ tự được chọn đã trùng một bài học khác. Vui lòng thử lại..");
                        return View(lesson);
                    }
                    existingLesson.Order = lesson.Order;
                    existingLesson.VideoUrl = lesson.VideoUrl;
                    existingLesson.FilePath = lesson.FilePath;
                    existingLesson.Status = lesson.Status;
                    existingLesson.Duration = lesson.Duration;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Bài học đã được cập nhật thành công.";
                    return RedirectToAction("Index", new { chapterId = lesson.ChapterId });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ModelState.AddModelError("", $"Unable to save changes. {ex.Message}");
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            return View(lesson);
        }

        #endregion Edit

        #region Delete
        // [HttpGet]
        // [Route("Delete")]
        // public async Task<IActionResult> Delete(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }

        //     var lesson = await _context.Lessons.FindAsync(id);
        //     if (lesson == null)
        //     {
        //         return NotFound();
        //     }

        //     return View(lesson);
        // }
        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }
            
            var progresses = _context.Progresses.Where(p => p.LessonId == id);
            _context.Progresses.RemoveRange(progresses);

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Bài học đã được xóa thành công.";
            return RedirectToAction("Index", new { chapterId = lesson.ChapterId });
        }
        #endregion Delete
        
    }
}