using APIs.DTOs.CourseDtos;
using APIs.Models;

namespace APIs.DTOs.TrainerDtos
{
    public class GetTrainerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public virtual ICollection<CourseDto>? Courses { get; set; }
    }
}
