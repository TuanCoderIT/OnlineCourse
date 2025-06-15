using System.ComponentModel.DataAnnotations;

namespace OnlineCourse.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string? UserId { get; set; }
        public string? Comment { get; set; }
        public int Rate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Course? Course { get; set; }
        public AppUserModel? User { get; set; }
    }
}