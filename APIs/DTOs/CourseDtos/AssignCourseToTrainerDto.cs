using System.ComponentModel.DataAnnotations;

namespace APIs.DTOs.CourseDtos
{
    public class AssignCourseToTrainerDto
    {
        [Required]
        public int CourseId { get; set; }
        [Required]
        public string TrainerEmail { get; set; }
    }
}
