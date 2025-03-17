using System.ComponentModel.DataAnnotations.Schema;

namespace APIs.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [ForeignKey(nameof(Trainer))]
        public string TrainerId { get; set; }
        public virtual Trainer Trainer { get; set; }
    }
}
