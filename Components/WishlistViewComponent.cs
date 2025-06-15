using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;

namespace OnlineCourse.Components
{
    [ViewComponent(Name = "Wishlist")]
    public class WishlistViewComponent : ViewComponent
    {
        private readonly DataContext _context;
        public WishlistViewComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var wishlist = await _context.Wishlists.Include(w => w.Course).ToListAsync();
            return await Task.FromResult((IViewComponentResult)View("Default", wishlist));
        }
    }
}