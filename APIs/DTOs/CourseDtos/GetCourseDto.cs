namespace APIs.DTOs.CourseDtos
{
    public class GetCourseDto : CourseDto
    {
        public int Id { get; set; }
        public string TrainerFirstName { get; set; }
        public string TrainerLastName { get; set; }
        public string TrainerEmail { get; set; }
        public string TrainerUserName { get; set; }
    }
}
