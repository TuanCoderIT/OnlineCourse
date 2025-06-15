using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineCourse.Models
{
    public class Wishlist
    {
        [Key]
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string? UserId { get; set; }
        public virtual Course? Course { get; set; }
        public virtual AppUserModel? User { get; set; }
    }
}