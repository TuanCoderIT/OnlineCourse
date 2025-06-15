using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;
using System.Web;

namespace OnlineCourse.Controllers
{
    public class DocumentController : Controller
    {
        private readonly DataContext _context;
        public DocumentController(DataContext context)
        {
            _context = context;
        }
        
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Documents.Where(a => a.Status == true).OrderBy(d => d.Id).ToListAsync());
        }


    }
}