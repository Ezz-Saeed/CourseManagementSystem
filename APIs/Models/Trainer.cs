namespace APIs.Models
{
    public class Trainer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual ICollection<Course>? Courses { get; set; }
    }
}
