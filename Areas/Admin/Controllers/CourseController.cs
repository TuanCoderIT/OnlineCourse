using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineCourse.Areas.Admin.Repository;
using OnlineCourse.Models;

namespace OnlineCourse.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Course")]
    [Authorize(Roles = "Admin")]
    public class CourseController : Controller
    {
        private readonly ICourseRepository _courseRepository;
        public CourseController(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var categories = _courseRepository.GetCategoriesNew();
            ViewBag.CategoryList = categories;
            return View(await _courseRepository.GetAllAsync());
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_courseRepository.GetCategories(), "Id", "Name");
            return View();
        }
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            if (ModelState.IsValid)
            {
                await _courseRepository.CreateAsync(course);
                TempData["Success"] = "Khoá học đã được thêm mới thành công!";
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(_courseRepository.GetCategories(), "Id", "Name", course.CategoryId);
            return View(course);
        }

        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            ViewBag.CategoryId = new SelectList(_courseRepository.GetCategories(), "Id", "Name", course.CategoryId);
            return View(course);
        }
        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                await _courseRepository.EditAsync(course);
                TempData["Success"] = "Khoá học đã được cập nhật thành công!";
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(_courseRepository.GetCategories(), "Id", "Name", course.CategoryId);
            return View(course);
        }

        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }
        [HttpPost]
        [Route("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _courseRepository.DeleteAsync(id);
            TempData["Success"] = "Khoá học đã được xóa thành công!";
            return RedirectToAction("Index");
        }
        
    }
}