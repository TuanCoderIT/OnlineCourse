using System.ComponentModel.DataAnnotations;

namespace OnlineCourse.Models
{
    public partial class Lesson
    {
        public int LessonId { get; set; }

        [Required]
        public string? Name { get; set; }
        public int Order { get; set; }
        public string? Details { get; set; }
        public string? ContentType { get; set; }
        public string? VideoUrl { get; set; }
        public string? FilePath { get; set; }
        public bool Status { get; set; }
        public int? ChapterId { get; set; }
        public virtual Chapter? Chapter { get; set; }
        public int? CourseId { get; set; }
        public virtual Course? Course { get; set; }
        public string? Duration { get; set; }
        public ICollection<Progresses>? Progresses { get; set; }

    }
}