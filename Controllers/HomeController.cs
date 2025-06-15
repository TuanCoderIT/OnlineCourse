using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace OnlineCourse.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly DataContext _context;
		private readonly UserManager<AppUserModel> _userManager;

		public HomeController(ILogger<HomeController> logger, DataContext context, UserManager<AppUserModel> userManager)
		{
			_logger = logger;
			_context = context;
			_userManager = userManager;
		}

		public IActionResult Index()
		{
			var courses = _context.Courses.Include("Category").Include(c => c.Lessons).ToList();
			return View(courses);
		}
		
		public async Task<IActionResult> AllCourses()
		{
			var allCourses = await _context.Courses.Include(c => c.Lessons).Where(x => x.Status == true).OrderBy(c => c.Id).ToListAsync();
			return View(allCourses);
		}

		[Authorize]
        public async Task<IActionResult> Profile()
        {
            return View(await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)));
        }

		[Authorize]
		public async Task<IActionResult> Wishlist()
		{
			return View(await _context.Wishlists.Include(w => w.Course).Where(w => w.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToListAsync());
		}
		
		[Authorize]
		[HttpPost]
		public async Task<IActionResult> AddWishlist(int Id)
		{
			if (await _context.Wishlists.Where(w => w.CourseId == Id && w.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)).FirstOrDefaultAsync() != null)
				return BadRequest("Khóa học đã có trong danh sách yêu thích.");

			var wishlistProduct = new Wishlist
			{
				CourseId = Id,
				UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
			};

			_context.Wishlists.Add(wishlistProduct);
			
			try
			{
				await _context.SaveChangesAsync();
				return Ok(new { success = true, message = "Add to wishlist Successfully" });
			}
			catch (Exception)
			{
				return StatusCode(500, "An error occurred while adding to wishlist");
			}
		}
		
		public async Task<IActionResult> RemoveWishlist(int Id)
		{
			Wishlist wishlistProduct = await _context.Wishlists.FindAsync(Id);
			if (wishlistProduct == null)
			{
				return NotFound();
			}
			_context.Wishlists.Remove(wishlistProduct);
			await _context.SaveChangesAsync();
			TempData["Success"] = "Xoá khỏi danh sách yêu thích thành công";
			return RedirectToAction("Wishlist");
		}

		// [Authorize]
		// public async Task<IActionResult> MyCourses()
		// {
		// 	var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		// 	var user = await _userManager.FindByIdAsync(userId);

		// 	if (user == null)
		// 	{
		// 		return NotFound();
		// 	}

		// 	var myCourses = await _context.Enrollments
		// 		.Include(e => e.Course)
		// 		.Where(e => e.UserId == userId)
		// 		.Select(e => e.Course)
		// 		.ToList();

		// 	return View(myCourses);
		// }

		// Edit Profile
		[Authorize]
		[HttpGet]
		public async Task<IActionResult> EditProfile()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var user = await _userManager.FindByIdAsync(userId);

			if (user == null)
			{
				return NotFound();
			}

			var model = new AppUserModel
			{
				Id = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber
			};

			return View(model);
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditProfile(AppUserModel model)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var user = await _userManager.FindByIdAsync(userId);

			if (user == null)
			{
				return NotFound();
			}

			user.UserName = model.UserName;
			user.Email = model.Email;
			user.PhoneNumber = model.PhoneNumber;

			var result = await _userManager.UpdateAsync(user);

			if (result.Succeeded)
			{
				return RedirectToAction("Profile");
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return View(model);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error(int statuscode)
		{
			if (statuscode == 404)
			{
				return View("404");
			}
			else
			{
				return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
			}
		}
	}
}
