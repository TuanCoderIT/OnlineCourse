using Microsoft.AspNetCore.Identity;

namespace OnlineCourse.Models
{
    public class AppUserModel : IdentityUser
    {
        public string? Occupation { get; set; }
        public string? RoleId { get; set; }
        public string? Avatar { get; set; }
    }
}