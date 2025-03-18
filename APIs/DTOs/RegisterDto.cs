using System.ComponentModel.DataAnnotations;

namespace APIs.DTOs
{
    public class RegisterDto
    {
        [StringLength(100)]
        public string FirstName { get; set; }
        [StringLength(100)]
        public string LastName { get; set; }
        [StringLength(100)]
        public string Email { get; set; }
        [StringLength(100)]
        public string Password { get; set; }
        [StringLength(50)]
        public string UserName { get; set; }
    }
}
