using Microsoft.AspNetCore.Identity;

namespace APIs.Models
{
    public class Appuser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsApproved { get; set; }
        public virtual ICollection<Course>? Courses { get; set; }
    }
}
