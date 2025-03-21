using System.ComponentModel.DataAnnotations;

namespace APIs.Data
{
    public class CourseTrainer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        //public string TrainerFirstName { get; set; }
        //public string TrainerLastName { get; set; }
        public string TrainerName { get; set; }
        public string TrainerEmail { get; set; }
        public string TrainerUserName { get; set; }
    }
}
