using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;

namespace OnlineCourse.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Profile")]
    [Authorize(Roles = "Admin")]
    public class ProfileController : Controller
    {
        private readonly DataContext _context;
        public ProfileController(DataContext context)
        {
            _context = context;
        }
        
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            // return View(await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.Name));
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // ID của người dùng hiện tại
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            return View(user);
        }

        
    }
}