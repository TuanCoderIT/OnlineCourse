using System.ComponentModel.DataAnnotations;

namespace OnlineCourse.Models.ViewModels
{
    public class RatingViewModel
    {
        public int? Id { get; set; }
        public int CourseId { get; set; }
        public int Rate { get; set; }
        [Required(ErrorMessage = "Bình luận không được để trống")]
        public string? Comment { get; set; }
    }
}