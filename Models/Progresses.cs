using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineCourse.Models
{
    public class Progresses
    {
        [Key]
        public int Id { get; set; }
        public int? LessonId { get; set; }
        public string? UserId { get; set; }
        public int? CourseId { get; set; }
        public bool? IsCompleted { get; set; }
        public Lesson? Lesson { get; set; }
    }
}