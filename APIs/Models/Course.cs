using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIs.Models
{
    public class Course
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public bool IsDeleted { get; set; }
        [ForeignKey(nameof(Trainer))]
        public string? TrainerId { get; set; }
        public virtual Appuser? Trainer { get; set; }
    }
}
