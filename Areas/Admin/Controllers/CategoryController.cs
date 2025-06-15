using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;

namespace OnlineCourse.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Category")]
	[Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly DataContext _context;
        public CategoryController(DataContext context)
        {
            _context = context;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.CourseCategories.OrderBy(c => c.Id).ToListAsync());
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
		[Route("Create")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CourseCategory category)
		{

			if (ModelState.IsValid)
			{
				_context.Add(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Danh mục đã được thêm thành công";
                return RedirectToAction(nameof(Index));
			}
			else
			{
				TempData["Error"] = "The category could not be added";
				List<string> errors = new List<string>();
				foreach (var modelState in ModelState.Values)
				{
					foreach (var error in modelState.Errors)
					{
						errors.Add(error.ErrorMessage);
					}
				}
				string errorMessage = string.Join("\n", errors);
				return BadRequest(errorMessage);
			}
			return View(category);
		}

        [HttpGet]
		[Route("Edit")]
		public async Task<IActionResult> Edit(int id)
		{
			var category = await _context.CourseCategories.FindAsync(id);
			if (category == null)
			{
				return NotFound();
			}
			return View(category);
		}
		[HttpPost]
		[Route("Edit")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(CourseCategory category)
		{
			if (ModelState.IsValid)
			{
				var existingCategory = await _context.CourseCategories.FindAsync(category.Id);
                if (existingCategory == null)
                {
                    return NotFound();
                }
                existingCategory.Name = category.Name;
				existingCategory.Description = category.Description;
				existingCategory.Slug = category.Slug;
				existingCategory.Status = category.Status;

				_context.CourseCategories.Update(existingCategory);
				await _context.SaveChangesAsync();
				TempData["Success"] = "Đã cập nhật danh mục thành công";
				return RedirectToAction(nameof(Index));
			}
			else
			{
				TempData["Error"] = "The category could not be updated";
				List<string> errors = new List<string>();
				foreach (var modelState in ModelState.Values)
				{
					foreach (var error in modelState.Errors)
					{
						errors.Add(error.ErrorMessage);
					}
				}
				string errorMessage = string.Join("\n", errors);
				return BadRequest(errorMessage);
			}
			return View(category);
		}

		[HttpGet]
		[Route("Delete")]
		public async Task<IActionResult> Delete(int id)
		{
			var category = await _context.CourseCategories.FindAsync(id);
			if (category == null)
			{
				return NotFound();
			}
			_context.CourseCategories.Remove(category);
			await _context.SaveChangesAsync();
			TempData["Success"] = "Danh mục đã được xóa thành công";
			return RedirectToAction(nameof(Index));
		}
    }
}