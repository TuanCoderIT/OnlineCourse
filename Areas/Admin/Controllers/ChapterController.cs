using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;

namespace OnlineCourse.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Chapter")]
    [Authorize(Roles = "Admin")]
    public class ChapterController : Controller
    {
        private readonly DataContext _context;
        public ChapterController(DataContext context)
        {
            _context = context;
        }

        #region Index
        // [Route("Index")]
        // public async Task<IActionResult> Index()
        // {
        //     return View(await _context.Chapters.ToListAsync());
        // }

        // [Route("Index/{courseId}")]
        [Route("Index")]
        public async Task<IActionResult> Index(int? courseId)
        {
            if (courseId == null)
            {
                return View(await _context.Chapters.ToListAsync());
            }
            var chapters = await _context.Chapters
                .Where(l => l.CourseId == courseId)
                .OrderBy(l => l.Order)
                .ToListAsync();

            return View(chapters);
        }
        #endregion Index

        #region Create
        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create(int courseId)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null)
            {
                ModelState.AddModelError("", "The specified course does not exist.");
                return RedirectToAction("Index", "Course");
            }

            var chapter = new Chapter { ChapterId = courseId };
            return View(chapter);
        }
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Chapter chapter)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == chapter.CourseId);

            if (course == null)
            {
                ModelState.AddModelError("", "The specified course does not exist.");
                return View(chapter);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var lastChapter = _context.Chapters
                        .Where(c => c.CourseId == chapter.CourseId)
                        .OrderByDescending(c => c.Order)
                        .FirstOrDefault();

                    chapter.Order = lastChapter != null ? lastChapter.Order + 1 : 1;

                    _context.Add(chapter);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Đã thêm chương mới thành công.";

                    return RedirectToAction("Index", new { chapter.CourseId });
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return View(chapter);
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

            var chapter = await _context.Chapters.FindAsync(id);
            if (chapter == null)
            {
                return NotFound();
            }
            return View(chapter);
        }
        [HttpPost]
        [Route("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Chapter chapter)
        {
            if (id != chapter.ChapterId)
            {
                return BadRequest("Invalid request");
            }

            var existChapter = await _context.Chapters.FindAsync(id);
            if (existChapter == null)
            {
                return NotFound("Chapter not found");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var isDuplicateOrder = _context.Chapters
                        .Any(c => c.CourseId == chapter.CourseId && c.Order == chapter.Order && c.ChapterId != id);

                    if (isDuplicateOrder)
                    {
                        ModelState.AddModelError("", "Thứ tự được chỉ định đã được gán cho một chương khác. Vui lòng thử lại.");
                        return View(chapter);
                    }

                    existChapter.Name = chapter.Name;
                    existChapter.Order = chapter.Order;
                    existChapter.Status = chapter.Status;

                    _context.Update(existChapter);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Đã cập nhật chương thành công.";
                    return RedirectToAction("Index", new { existChapter.CourseId });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ModelState.AddModelError("", $"Unable to save changes. {ex.Message}");
                }
            }
            return View(chapter);
        }
        #endregion Edit

        #region Delete
        [HttpGet]
        [Route("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapter = await _context.Chapters.FindAsync(id);
            if (chapter == null)
            {
                return NotFound();
            }

            return View(chapter);
        }
        [HttpPost, ActionName("Delete")]
        [Route("Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Chapter chapter)
        {
            var delChapter = await _context.Chapters.FindAsync(id);
            if (delChapter == null)
            {
                return NotFound();
            }
            var courseId = delChapter.CourseId;
            _context.Chapters.Remove(delChapter);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa chương thành công.";
            return RedirectToAction("Index", new { courseId });
        }
        #endregion Delete

        
    }
}