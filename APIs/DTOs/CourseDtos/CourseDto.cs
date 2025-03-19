using System.ComponentModel.DataAnnotations;

namespace APIs.DTOs.CourseDtos
{
    public class CourseDto
    {
        [MaxLength(150)]
        [Required]
        public string Name { get; set; }
        [MaxLength(150)]
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
}
